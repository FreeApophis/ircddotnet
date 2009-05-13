/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 19.03.2009
 * Zeit: 15:18
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace IrcD
{
	class UserPerChannelInfo {
        public UserPerChannelInfo(UserInfo info) {
            this.info = info;
        }
            
        private UserInfo info;
        
        public UserInfo Info {
            get { 
                return info; 
            }
        }
        
        public string NickPrefix {
            get {
                if (mode_o) {
                    return "@";
                } else if (mode_h) {
                    return "%";
                } else if (mode_v) {
                    return "+";
                } else {
                    return "";
                }
            }
        }
        
        private bool mode_v;
	    
        /// <summary>
        /// Is User Voiced
        /// </summary>
	    public bool Mode_v {
	        get {
	            return mode_v;
	        }
	        set { 
	            mode_v = value;
	        }
	    }
        
        private bool mode_h;
        
        /// <summary>
        /// Is User Half-Op
        /// </summary>
        public bool Mode_h {
            get { 
                return mode_h; 
            }
            set { 
                mode_h = value; 
            }
        }
        
	    
	    private bool mode_o;
	    
	    /// <summary>
	    /// Is User Op
	    /// </summary>
	    public bool Mode_o {
	        get { 
	            return mode_o; 
	        }
	        set { 
	            mode_o = value; 
	        }
	    }
	}
}
