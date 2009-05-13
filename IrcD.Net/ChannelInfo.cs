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
    class ChannelInfo {
        public ChannelInfo(string name) {
            this.name = name;
        }
        
        private string name;
        
        public string Name {
            get { 
                return name; 
            }
        }       
        
        private string topic;
        
        public string Topic {
            get {
                return topic;
            }
            set {
                topic = value;
            }
        }
        
        private Dictionary<string, UserPerChannelInfo> user = new Dictionary<string, UserPerChannelInfo>();
        
        public Dictionary<string, UserPerChannelInfo> User {
            get {
                return user;
            }
        }

        private bool mode_a;
        
        /// <summary>
        /// Anonymous Channel Flag
        /// </summary>
        public bool Mode_a {
            get { 
                return mode_a; 
            }
            set { 
                mode_a = value;
            }
        }
        
        private List<string> mode_b = new List<string>();
        
        /// <summary>
        /// Ban list
        /// </summary>
        public List<string> Mode_b {
            get {
                return mode_b; 
            }
        }
        
        private List<string> mode_e = new List<string>();
        
        /// <summary>
        /// Ban except list
        /// </summary>
        public List<string> Mode_e {
            get { 
                return mode_e; 
            }
        }
        
        private bool mode_i;
        
        /// <summary>
        /// Invite Only Channel Flag
        /// </summary>
        public bool Mode_i {
            get { 
                return mode_i;
            }
            set { 
                mode_i = value; 
            }
        }
        
        private List<string> mode_I = new List<string>();
        
        /// <summary>
        /// Invite List
        /// </summary>
        public List<string> Mode_I {
            get { 
                return mode_I; 
            }
        }
        
        private string mode_k;
        
        /// <summary>
        /// Channel Key
        /// </summary>
        public string Mode_k {
            get { 
                return mode_k;
            }
            set { 
                mode_k = value; 
            }
        }
        
        private int mode_l = -1;
        
        /// <summary>
        /// Channel Limit
        /// </summary>
        public int Mode_l {
            get { 
                return mode_l; 
            }
            set {
                mode_l = value; 
            }
        }
        
        private bool mode_m;
        
        /// <summary>
        /// Moderated Channel Flag
        /// </summary>
        public bool Mode_m {
            get { 
                return mode_m; 
            }
            set { 
                mode_m = value; 
            }
        }
        
        private bool mode_n;
        
        /// <summary>
        ///  No Messages To Channel From Clients On The Outside
        /// </summary>
        public bool Mode_n {
            get { 
                return mode_n; 
            }
            set { 
                mode_n = value;
            }
        }

        private bool mode_p;
        
        /// <summary>
        /// Private Flag
        /// </summary>
        public bool Mode_p {
            get { 
                return mode_p;
            }
            set { 
                mode_p = value;
            }
        }
        
        private bool mode_q;
        
        /// <summary>
        /// Quiet Flag
        /// </summary>
        public bool Mode_q {
            get { 
                return mode_q; 
            }
            set { 
                mode_q = value;
            }
        }
        private bool mode_r;
        
        /// <summary>
        /// Server Reop Flag
        /// </summary>
        public bool Mode_r {
            get { return mode_r; }
            set { mode_r = value; }
        }
        
        private bool mode_s;
        
        /// <summary>
        /// Secret Flag
        /// </summary>
        public bool Mode_s {
            get { 
                return mode_s; 
            }
            set { 
                mode_s = value; 
            }
        }
        
        private bool mode_t;
        
        /// <summary>
        /// Topic Restricted Flag
        /// </summary>
        public bool Mode_t {
            get { 
                return mode_t; 
            }
            set { 
                mode_t = value;
            }
        }
    }
}
