using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    // Trata os argumentos para o evento StartChanged

    internal class StatusChangedEventArgs : EventArgs
    {
        private String EventMsg;

        public String EventMessage
        { 
            get { return EventMsg; }
            set { EventMsg = value; }
        }

        public StatusChangedEventArgs(string strEventMsg) 
        { 
            EventMsg = strEventMsg;
        }
    }
}
