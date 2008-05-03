<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" indent="yes" encoding="US-ASCII"/>
<xsl:template match="/">
  <a>
    <xsl:attribute name="href">https://www.codeplex.com/Release/ProjectReleases.aspx?ProjectName=ccnetplugins&amp;ReleaseId=<xsl:value-of select="CodePlexRelease/@Id"/></xsl:attribute>
    <xsl:attribute name="target">_blank</xsl:attribute>
    CodePlex Release <xsl:value-of select="CodePlexRelease/@Name"/> <xsl:value-of select="CodePlexRelease/@Type"/>
  </a>
</xsl:template>
</xsl:stylesheet>