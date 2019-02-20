using System;
using System.Collections.Generic;
using System.Linq;
using YetAnotherRelogger.Helpers.Hotkeys.Actions;

namespace YetAnotherRelogger.Helpers.Hotkeys
{
    public class ActionContainer
    {
        private static HashSet<IHotkeyAction> _actionList;

        static ActionContainer()
        {
            _actionList = new HashSet<IHotkeyAction>
            {
                new RepositionAll(),
                new RepositionCurrent(),
                new FullScreen(),
                new ResizeCurrent()
            };
            // Create list
        }

        public ActionContainer()
        {
            _actionList = new HashSet<IHotkeyAction>
            {
                new RepositionAll(),
                new RepositionCurrent(),
                new FullScreen(),
                new ResizeCurrent()
            };
            // Create list

            // Create Name and Version list
            Actions = new List<Action>();
            foreach (var a in _actionList)
            {
                var action = new Action {Name = a.Name, Version = a.Version, Description = a.Description};
                Actions.Add(action);
            }
        }

        public List<Action> Actions { get; }

        /// <summary>
        ///     Get Action by name and version
        /// </summary>
        /// <param name="name">Action Name</param>
        /// <param name="version">Action Version</param>
        /// <returns>returns object</returns>
        public static IHotkeyAction GetAction(string name, Version version)
        {
            var ret = _actionList.FirstOrDefault(x => x.Name == name && x.Version == version);
            return ret;
        }
    }
}
