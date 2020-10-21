/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2008 by AG-Software 											 *
 * All Rights Reserved.																 *
 * Contact information for AG-Software is available at http://www.ag-software.de	 *
 *																					 *
 * Licence:																			 *
 * The agsXMPP SDK is released under a dual licence									 *
 * agsXMPP can be used under either of two licences									 *
 * 																					 *
 * A commercial licence which is probably the most appropriate for commercial 		 *
 * corporate use and closed source projects. 										 *
 *																					 *
 * The GNU Public License (GPL) is probably most appropriate for inclusion in		 *
 * other open source projects.														 *
 *																					 *
 * See README.html for details.														 *
 *																					 *
 * For general enquiries visit our website at:										 *
 * http://www.ag-software.de														 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.vcard
{
    /// <summary>
    /// </summary>
    public class Organization : Element
    {
        #region << Constructors >>

        public Organization()
        {
            TagName = "ORG";
            Namespace = Uri.VCARD;
        }

        public Organization(string name, string unit) : this()
        {
            Name = name;
            Unit = unit;
        }

        #endregion

        // <ORG>
        //	<ORGNAME>Jabber Software Foundation</ORGNAME>
        //	<ORGUNIT/>
        // </ORG>

        public string Name
        {
            get { return GetTag("ORGNAME"); }
            set { SetTag("ORGNAME", value); }
        }

        public string Unit
        {
            get { return GetTag("ORGUNIT"); }
            set { SetTag("ORGUNIT", value); }
        }
    }
}