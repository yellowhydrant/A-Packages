using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem;

namespace A.DebugConsole
{
    public class ADebugConsole : MonoBehaviour
    {
        static Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
        static List<string> log = new List<string>();

        const string ErrorMessageInvalidCommand = "Command not found or invalid!";
        const string ErrorMessageInvalidArgument = "Argument invalid, command has not been executed!";

        static ADebugConsoleInput input;

        static NTree<string> commandTree;
        static NTree<string> invalidTree = new NTree<string>("Invalid Command!");

        public NTree<string> tree;

        bool isConsoleOpen;
        string currentInput;
        string previousInput;
        List<NTree<string>> suggestions = new List<NTree<string>>();

        GUIStyle style;

        private void Awake()
        {
            if(input == null)
                input = new ADebugConsoleInput();

            //default commands
            //commands.Add("print", new ConsoleFunc() { name = "print", func = (msg) => { Debug.Log(msg); return msg.ToString(); } });

            commands.Add("debug.print", new ConsoleCommand());
            commands.Add("debug.cheat.log", new ConsoleCommand());
            commands.Add("debug.cheat.error", new ConsoleCommand());
            commands.Add("player.movetopos", new ConsoleCommand());
            commands.Add("player.inventory.additem", new ConsoleCommand());
            commands.Add("player.inventory.removeitem", new ConsoleCommand());
            RebuildTree();
            tree = commandTree;
            //add log and make it so that the output gets printen into log //change everything to new ui system//fix closing when typing

            DontDestroyOnLoad(gameObject);
        }

        #region Input
        private void OnEnable()
        {
            input.Enable();
            input.Console.OpenClose.started += OpenClose;
            input.Console.Execute.started += Execute;
        }

        private void OnDisable()
        {
            if (input == null) return;
            input.Disable();
            input.Console.OpenClose.started -= OpenClose;
            input.Console.Execute.started -= Execute;
        }

        void OpenClose(InputAction.CallbackContext _) => isConsoleOpen = !isConsoleOpen;
        void Execute(InputAction.CallbackContext _) => Debug.Log(ExecuteCommand(currentInput));
        #endregion

        void OnGUI()
        {
            if(style == null)
                style = new GUIStyle(GUI.skin.GetStyle("textField"));
            if (isConsoleOpen)
            {
                style.focused.textColor = IsValidCommand(currentInput) ? Color.green : Color.red;
                currentInput = GUILayout.TextField(currentInput, style, GUILayout.MinWidth(Screen.width), GUILayout.MinHeight(20f), GUILayout.ExpandHeight(false));
                if(currentInput != previousInput)
                {
                    previousInput = currentInput;
                    UpdateSuggestions();
                }
                for (int i = 0; i < suggestions.Count; i++)
                {
                    GUILayout.Label(suggestions[i].data);
                }
            }
        }

        void UpdateSuggestions()
        {
            var spaces = currentInput.Split(' ')[0].Split('.');
            if (spaces.Length <= 1)
            {
                suggestions = commandTree.children;
            }
            else
            {
                var tree = commandTree;
                for (int i = 0; i < spaces.Length - 1; i++)
                {
                    tree = tree.children.FirstOrDefault((x) => x.data == spaces[i]);
                    if (tree == null)
                    {
                        tree = invalidTree;
                        invalidTree.AddChild("Command not found!");
                        break;
                    }
                }
                suggestions = tree.children;
            }
                

        }

        [MyBox.ButtonMethod]
        static void RebuildTree()
        {
            commandTree = new NTree<string>("root");
            var spaces = commands.Keys.ToArray();
            var splitCommands = new List<string>[spaces.Length];
            for (int i = 0; i < spaces.Length; i++)
                splitCommands[i] = new List<string>(spaces[i].Split('.'));
            var levelAmount = spaces.Max((_) => _.Split('.').Length);
            //sort commands into unique levels
            var levels = new HashSet<string>[levelAmount];
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = new HashSet<string>();
                for (int j = 0; j < splitCommands.Length; j++)
                {
                    if (splitCommands[j].Count > i)
                        levels[i].Add(splitCommands[j][i]);
                }
                #region Debug
                //debugging
                //var str = new System.Text.StringBuilder("[");
                //var arr = new List<string>(levels[i]);
                //for(int j = 0; j < arr.Count; j++)
                //{
                //    str.Append(arr[j]);
                //    if(j < arr.Count - 1)
                //    str.Append(" ,");
                //}
                //str.Append(']');
                //Debug.Log(str);
                #endregion
            }

            var children = new List<NTree<string>>() { commandTree };
            var nextChildren = new List<NTree<string>>();
            //create a tree structure
            for (int i = 0; i < levels.Length; i++)//every level
            {
                var arr = levels[i].ToArray();
                for (int k = 0; k < children.Count; k++)//every child
                {
                    for (int j = 0; j < arr.Length; j++)//every space
                    {
                        //check any split commands have this space on their 
                        var relation = splitCommands.Any((x) =>
                        {
                            if (i >= x.Count)
                                return false;
                            else if (i == 0)
                                return x[i] == arr[j];
                            else
                                return x[i] == arr[j] && x[i - 1] == children[k].data;
                        });
                        if (relation)
                        {
                            var child = children[k].AddChild(arr[j]);
                            nextChildren.Add(child);
                        }
                    }
                }
                children = nextChildren;
                nextChildren = new List<NTree<string>>();
            }

        }

        #region CommandWrappers
        public class ConsoleCommand
        {
            public int requiredAccessLevel;
            public string name;
            public string description;
            public object target;
        }

        class ConsoleMethod : ConsoleCommand
        {
            public MethodInfo info;
        }

        class ConsoleProperty : ConsoleCommand
        {
            public PropertyInfo info;
        }

        class ConsoleField : ConsoleCommand
        {
            public FieldInfo info;
        }

        class ConsoleFunc : ConsoleCommand
        {
            public System.Func<object, string> func;
        }

        class InvalidArgument
        {
            public static readonly InvalidArgument Instance = new InvalidArgument();
        }
        #endregion

        #region Static
        public static List<ConsoleCommand> AddFromObject(string root, Object target)
        {
            var addedCommands = new List<ConsoleCommand>();
            var type = target.GetType();
            var members = type.GetMembers().Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(ADebugInputAttribute))).ToArray();
            
            for (int i = 0; i < members.Length; i++)
            {
                var attribute = members[i].GetCustomAttribute<ADebugInputAttribute>();
                var name = $"{root}.{target.name}.{attribute.name}";
                ConsoleCommand command = null;
                if (attribute is ADebugMethodAttribute)
                    command = new ConsoleMethod { name = attribute.name, description = attribute.description, requiredAccessLevel = attribute.requiredAccessLevel, target = target };
                else if (attribute is ADebugPropertyAttribute)
                    command = new ConsoleProperty { name = attribute.name, description = attribute.description, requiredAccessLevel = attribute.requiredAccessLevel, target = target };
                else if (attribute is ADebugFieldAttribute)
                    command = new ConsoleField { name = attribute.name, description = attribute.description, requiredAccessLevel = attribute.requiredAccessLevel, target = target };
                if(commands.TryAdd(name, command))
                    addedCommands.Add(command);
            }

            return addedCommands;
        }

        public static void AddManual(ConsoleCommand command)
        {
            commands.Add(command.name, command);
        }

        static bool IsValidCommand(string input)
        {
            if (input == null)
                return false;
            var parts = input.Split(' ');
            return commands.Keys.Contains(parts[0]);
        }

        public static string ExecuteCommand(string input)
        {
            if (!IsValidCommand(input))
                return ErrorMessageInvalidCommand;
            //command example: inventory.fillslot 
            var parts = input.Split(' ');
            //for (int i = 0; i < parts.Length; i++)
            //    Debug.Log(parts[i]);
            var command = commands[parts[0]];
            var mode = parts.Length > 1 ? parts[1] : null;

            if (parts.Length == 1)
            {
                
                ParseArgument(input);
            }

            //make it versatile using dynamic invoke
            if(command is ConsoleFunc action) 
            {
                var arg = ParseArgument(mode);
                if (arg == InvalidArgument.Instance)
                    return ErrorMessageInvalidArgument;
                else
                    return action.func.Invoke(arg);
            }

            if (command is ConsoleMethod method)
            {
                if(mode == "list")
                {
                    var methodParams = method.info.GetParameters();
                    var output = "";
                    for (int i = 0; i < methodParams.Length; i++)
                    {
                        output += $"{methodParams[i].Name}({methodParams[i].ParameterType.Name}){(i == 0 ? null : "\n")}";
                    }
                    return output;
                }
                if(mode == "desc")
                {
                    return method.description;
                }
                else //default mode is call
                {
                    object[] parameters = new object[parts.Length - 2];

                    for (int i = 2; i < parts.Length; i++)
                    {
                        parameters[i - 2] = ParseArgument(parts[i]);
                        if (parameters[i - 2] == InvalidArgument.Instance)
                        {
                            return ErrorMessageInvalidArgument;
                        }
                    }

                    return method.info.Invoke(method.target, parameters).ToString();
                }
            }

            if (command is ConsoleProperty property)
            {
                if (mode == null)
                {
                    return property.info.GetValue(property.target).ToString();
                }
                else
                {
                    var val = ParseArgument(mode);
                    if (val != InvalidArgument.Instance)
                    {
                        property.info.SetValue(property.target, ParseArgument(mode));
                        return property.info.GetValue(property.target).ToString();
                    }
                    else
                    {
                        return ErrorMessageInvalidArgument;
                    }
                }
            }

            if (command is ConsoleField field)
            {
                if (mode == null)
                {
                    return field.info.GetValue(field.target).ToString();
                }
                else
                {
                    var val = ParseArgument(mode);
                    if (val != InvalidArgument.Instance)
                    {
                        field.info.SetValue(field.target, ParseArgument(mode));
                        return field.info.GetValue(field.target).ToString();
                    }
                    else
                    {
                        return ErrorMessageInvalidArgument;
                    }
                }
            }

            return ErrorMessageInvalidCommand;
        }

        static object ParseArgument(string arg)
        {
            if (arg == "null")
                return null;
            else if (arg.First() == '{' && arg.Last() == '}')
            {
                var rawValues = arg.Substring(1, arg.Length - 2).Split(',');
                var vec = new Vector4();
                for (int i = 0; i < rawValues.Length; i++)
                    if (ExpressionEvaluator.Evaluate<float>(rawValues[i], out var val))
                        vec[i] = val;
                switch (rawValues.Length)
                {
                    case 2:
                        return (Vector2)vec;
                    case 3:
                        return (Vector3)vec;
                    case 4:
                        return vec;
                    default:
                        return InvalidArgument.Instance;
                }
            }
            else if (arg.First() == '[' && arg.Last() == ']')
            {
                var rawValues = arg.Substring(1, arg.Length - 2).Split(',');
                var arr = new object[rawValues.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = ParseArgument(rawValues[i]);
                }
                if (arr.Length == 0)
                    return InvalidArgument.Instance;
                else
                    return arr;
            }
            else if (arg.First() == '"' && arg.Last() == '"')
                return arg.Substring(1, arg.Length - 2);
            else if (ExpressionEvaluator.Evaluate<float>(arg, out var val))
                return val;
            else
                return InvalidArgument.Instance;
        }
        #endregion
    }
}
