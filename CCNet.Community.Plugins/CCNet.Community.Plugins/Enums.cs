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

namespace CCNet.Community.Plugins {
  /// <summary>
  /// The status of the release
  /// </summary>
  public enum ReleaseStatus {
		/// <summary>
		/// The release is a planned release.
		/// </summary>
		Planned = 0,
		/// <summary>
		/// The release is released (stable?)
		/// </summary>
		Released,
		/// <summary>
		/// The release is in the planning stages
		/// </summary>
		Planning,
		/// <summary>
		/// The release is an alpha
		/// </summary>
		Alpha,
		/// <summary>
		/// The release is a beta
		/// </summary>
		Beta,
		/// <summary>
		/// The release is a stable release
		/// </summary>
		Stable,
  }

  /// <summary>
  /// The file type
  /// </summary>
  public enum ReleaseFileType {
    /// <summary>
    /// Binary file
    /// </summary>
    RuntimeBinary,
    /// <summary>
    /// Source Code file
    /// </summary>
    SourceCode,
    /// <summary>
    /// Documentation file
    /// </summary>
    Documentation,
    /// <summary>
    /// Example file
    /// </summary>
    Example
  }

  /// <summary>
  /// The type of release
  /// </summary>
  public enum ReleaseType {
    /// <summary>
    /// No specified release type
    /// </summary>
    None = 0,
    /// <summary>
    /// An alpha release
    /// </summary>
    Alpha,
    /// <summary>
    /// A beta release
    /// </summary>
    Beta,
    /// <summary>
    /// A nightly release
    /// </summary>
    Nightly,
    /// <summary>
    /// A production release
    /// </summary>
    Production
  }

  /// <summary>
  /// Supported Build Conditions
  /// </summary>
  public enum PublishBuildCondition : int {
    /// <summary>
    /// Build because of modifications
    /// </summary>
    IfModificationExists = 0,
    /// <summary>
    /// Forced Build
    /// </summary>
    ForceBuild,
    /// <summary>
    /// Any Build type.
    /// </summary>
    AllBuildConditions
  }

	public enum PublishBuildStatus : int {
		/// <summary>
		/// Build failed
		/// </summary>
		Failure = 0,
		/// <summary>
		/// Success
		/// </summary>
		Success,
		/// <summary>
		/// Any Build status.
		/// </summary>
		Any
	}

  /// <summary>
  /// Return only the coverage data requested.
  /// </summary>
  public enum NCoverCoverageType {
    /// <summary>
    /// All coverage data
    /// </summary>
    All = -1,
    /// <summary>
    /// only shows methods
    /// </summary>
    None = 0,
    /// <summary>
    /// shows sequence point data as well as methods
    /// </summary>
    SequencePoint = 1,
    /// <summary>
    /// shows branch point data as well as methods. This is only available in the Enterprise edition.
    /// </summary>
    Branch = 2
  }

  [Flags]
  public enum NCoverSymbolSearchPolicy {
    Default = 0,
    /// <summary>
    /// Queries the registry for symbol search paths
    /// </summary>
    AllowRegistryAccess = 1,
    /// <summary>
    /// Accesses a symbol server
    /// </summary>
    AllowSymbolServerAccess = 2,
    /// <summary>
    /// Searches the path speicifed in the Debug directory
    /// </summary>
    AllowOriginalPathAccess = 4,
    /// <summary>
    /// Searches for the PDB in the place where the .exe file is
    /// </summary>
    AllowReferencePathAccess = 8
  }
}
