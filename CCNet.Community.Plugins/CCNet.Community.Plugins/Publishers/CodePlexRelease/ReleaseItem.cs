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
using Exortech.NetReflector;

namespace CCNet.Community.Plugins.Publishers {
  [ReflectorType ( "release" )]
  public class ReleaseItem : ReleaseBase {
    private string description;
    private bool isDefaultRelease;
    private DateTime? releaseDate;
    private ReleaseStatus releaseStatus = ReleaseStatus.Beta;
    private bool showOnHomePage;
    private bool showToPublic;
    private List<ReleaseFile> files = null;
    private ReleaseType releaseType = ReleaseType.None;
    private PublishBuildCondition _buildCondition = PublishBuildCondition.AllBuildConditions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseItem"/> class.
    /// </summary>
    public ReleaseItem ( ) {
      files = new List<ReleaseFile> ( );
    }
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    [ReflectorProperty ( "description", Required = true )]
    public string Description {
      get { return this.description; }
      set { this.description = value; }
    }

    /// <summary>
    /// Gets or sets the files.
    /// </summary>
    /// <value>The files.</value>
    [ReflectorArray ( "releaseFiles",Required = false )]
    public List<ReleaseFile> Files { get { return this.files; } set { this.files = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is default release.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance is default release; otherwise, <see langword="false"/>.
    /// </value>
    [ReflectorProperty ( "isDefaultRelease", Required = true )]
    public bool IsDefaultRelease {
      get { return this.isDefaultRelease; }
      set { this.isDefaultRelease = value; }
    }

    /// <summary>
    /// Gets or sets the release date.
    /// </summary>
    /// <value>The release date.</value>
    [ReflectorProperty ( "releaseDate", Required = false )]
    public DateTime ReleaseDate {
      get {
        if ( !this.releaseDate.HasValue )
          return DateTime.Now;
        return this.releaseDate.Value;
      }
      set { this.releaseDate = new DateTime? ( value ); }
    }

    /// <summary>
    /// Gets or sets the release status.
    /// </summary>
    /// <value>The release status.</value>
    [ReflectorProperty ( "releaseStatus", Required = false )]
    public ReleaseStatus Status {
      get { return this.releaseStatus; }
      set { this.releaseStatus = value; }
    }

    /// <summary>
    /// Gets or sets the type of the release.
    /// </summary>
    /// <value>The type of the release.</value>
    [ReflectorProperty("releaseType",Required=false)]
    public ReleaseType ReleaseType {
      get { return this.releaseType; }
      set { this.releaseType = value; } 
    }

    /// <summary>
    /// Gets or sets the build condition.
    /// </summary>
    /// <value>The build condition.</value>
    [ReflectorProperty ( "buildCondition", Required = false )]
    public PublishBuildCondition BuildCondition { get { return this._buildCondition; } set { this._buildCondition = value; } }



    /// <summary>
    /// Gets or sets a value indicating whether show on home page.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if show on home page; otherwise, <see langword="false"/>.
    /// </value>
    [ReflectorProperty ( "showOnHomePage", Required = true )]
    public bool ShowOnHomePage {
      get { return this.showOnHomePage; }
      set { this.showOnHomePage = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether show to public.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if show to public; otherwise, <see langword="false"/>.
    /// </value>
    [ReflectorProperty ( "showToPublic", Required = true )]
    public bool ShowToPublic {
      get { return this.showToPublic; }
      set { this.showToPublic = value; }
    }
  }
}
