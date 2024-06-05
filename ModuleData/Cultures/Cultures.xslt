

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output omit-xml-declaration="yes"/>
<xsl:template match="@*|node()">
    <xsl:copy>
        <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
</xsl:template>
 
    
    <xsl:template match="SPCultures[@id='sturgia']"/>
    <xsl:template match="SPCultures[@id='khuzait']"/>
    <xsl:template match="SPCultures[@id='aserai']"/>
    <xsl:template match="SPCultures[@id='battania']"/>
    <xsl:template match="SPCultures[@id='empire']"/>
    <xsl:template match="SPCultures[@id='vlandia']"/>
    <xsl:template match="SPCultures[@id='nord']"/>
    <xsl:template match="SPCultures[@id='vakken']"/>
    <xsl:template match="SPCultures[@id='darshi']"/>
    <xsl:template match="SPCultures[@id='looters']"/>
    <xsl:template match="SPCultures[@id='sea_raiders']"/>
    <xsl:template match="SPCultures[@id='mountain_bandits']"/>
    <xsl:template match="SPCultures[@id='forest_bandits']"/>
    <xsl:template match="SPCultures[@id='desert_bandits']"/>
    <xsl:template match="SPCultures[@id='steppe_bandits']"/>
    <xsl:template match="SPCultures[@id='neutral_culture']"/>
</xsl:stylesheet>

