using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace sldc.Commands
{
    /// <summary>
    /// Base to scaffold out core functionality of Commands
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public abstract void Execute(object? parameter);

        protected void OnCanExecutedChanged()
        { 
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
