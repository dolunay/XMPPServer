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

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    public class List : Element
    {
        public List()
        {
            TagName = "list";
            Namespace = Uri.IQ_PRIVACY;
        }

        public List(string name) : this()
        {
            Name = name;
        }

        public string Name
        {
            get { return GetAttribute("name"); }
            set { SetAttribute("name", value); }
        }

        /// <summary>
        ///   Gets all Rules (Items) when available
        /// </summary>
        /// <returns> </returns>
        public Item[] GetItems()
        {
            ElementList el = SelectElements(typeof (Item));
            int i = 0;
            var result = new Item[el.Count];
            foreach (Item itm in el)
            {
                result[i] = itm;
                i++;
            }
            return result;
        }

        /// <summary>
        ///   Adds a rule (item) to the list
        /// </summary>
        /// <param name="itm"> </param>
        public void AddItem(Item item)
        {
            AddChild(item);
        }

        public void AddItems(Item[] items)
        {
            foreach (Item item in items)
            {
                AddChild(item);
            }
        }

        /// <summary>
        ///   Remove all items/rules of this list
        /// </summary>
        public void RemoveAllItems()
        {
            RemoveTags(typeof (Item));
        }
    }
}