/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * The core interface for the Command design pattern implementation. 
 * It defines the standard Execute method that all command-based actions must implement, 
 * enabling a decoupled architecture between the UI and the underlying functional logic.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    internal interface ICommandButton
    {
        void Execute();
    }
}
