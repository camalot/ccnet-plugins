/*
 * http://www.codeplex.com/ccnetplugins/
 * 
 * Microsoft Public License (Ms-PL)
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not 
 * accept the license, do not use the software.
 * 
 * 1. Definitions
 * The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. 
 * copyright law.
 * 
 * A "contribution" is the original software, or any additions or changes to the software.
 * 
 * A "contributor" is any person that distributes its contribution under this license.
 * 
 * "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 * 
 * 2. Grant of Rights
 * 
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
 * each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
 * prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 * 
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each 
 * contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, 
 * sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the 
 * contribution in the software.
 * 
 * 3. Conditions and Limitations
 * 
 * (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
 * 
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your 
 * patent license from such contributor to the software ends automatically.
 * 
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices 
 * that are present in the software.
 * 
 * (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a 
 * complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code 
 * form, you may only do so under a license that complies with this license.
 * 
 * (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees 
 * or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent 
 * permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular 
 * purpose and non-infringement.
 */
using System;
using System.Collections.Generic;
using System.Text;
using CookComputing.XmlRpc;
using CCNet.Community.Plugins.XmlRpc;

namespace CCNet.Community.Plugins.Components.XmlRpc {
  public class MetaWeblogClient : XmlRpcClientProtocol, IMetaWeblog {
    #region IMetaWeblog Members
    [XmlRpcMethod ( "metaWeblog.newPost" )]
    public string newPost ( string blogid, string username, string password, Post content, bool publish ) {
      return ( string ) this.Invoke ( "newPost", new object[ ] { blogid, username, password, content, publish } );
    }

    [XmlRpcMethod ( "blogger.getUsersBlogs" )]
    public BlogInfo[ ] getUsersBlogs ( string appKey, string username, string password ) {
      return (BlogInfo[ ])this.Invoke ( "getUsersBlogs", new object[ ] { appKey, username, password } );
    }

    [XmlRpcMethod ( "metaWeblog.getCategories" )]
    public CCNet.Community.Plugins.XmlRpc.Category[ ] getCategories ( string blogid, string username, string password ) {
      return ( CCNet.Community.Plugins.XmlRpc.Category[ ] ) this.Invoke ( "getCategories", new object[ ] { blogid, username, password } );
    }

    [XmlRpcMethod ( "wpLinkMentor.getLinks" )]
    public Link[ ] getLinks ( string blogid,string username,string password, string catid ) {
      return (Link[ ]) this.Invoke ( "getLinks", new object[ ] { blogid, username, password, catid } );
    }

    [XmlRpcMethod ( "metaWeblog.editPost" )]
    public bool editPost ( string postid, string username, string password, Post post, bool publish ) {
      return ( bool ) this.Invoke ( "editPost", new object[ ] { postid, username, password, post, publish } );
    }
    [XmlRpcMethod ( "metaWeblog.getPost" )]
    public Post getPost ( string postid, string username, string password ) {
      return (Post)this.Invoke ( "getPost", new object[ ] { postid, username, password } );
    }
    [XmlRpcMethod ( "metaWeblog.getRecentPosts" )]
    public Post[ ] getRecentPosts ( string blogid, string username, string password, int numberOfPosts ) {
      return this.Invoke ( "getRecentPosts", new object[ ] { blogid, username, password, numberOfPosts } ) as Post[ ];
    }

    [XmlRpcMethod("metaWeblog.newMediaObject")]
    public mediaObjectInfo newMediaObject ( object blogid, string username, string password, mediaObject mediaobject ) {
      return (mediaObjectInfo) this.Invoke ( "newMediaObject", new object[ ] { blogid, username, password, mediaobject } );
    }
    [XmlRpcMethod ( "blogger.deletePost" )]
    public bool deletePost ( string appKey, string postid, string username, string password, bool publish ) {
      return ( bool ) this.Invoke ( "deletePost", new object[ ] { appKey, postid, username, password, publish } );
    }

    #endregion
  }
}
