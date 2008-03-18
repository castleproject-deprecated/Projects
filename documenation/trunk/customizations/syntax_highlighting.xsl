<?xml version="1.0" encoding="utf-8"?>
<!--
	Provides Syntax Highligting.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xslthl="http://xslthl.sf.net"
                exclude-result-prefixes="xslthl"
                version="1.0">

<xsl:param name="highlight.source" select="1"/>

<!-- 
<xsl:template match="programlisting" mode="class.value">
  <xsl:value-of select="@language"/>
</xsl:template>
-->
  
<xsl:template match='xslthl:keyword'>
  <span class="keyword"><xsl:apply-templates/></span>
</xsl:template>

<xsl:template match='xslthl:string'>
  <span class="string"><xsl:apply-templates/></span>
</xsl:template>

<xsl:template match='xslthl:comment'>
  <span class="comment"><xsl:apply-templates/></span>
</xsl:template>

<xsl:template match='xslthl:tag'>
  <span class="tag"><xsl:apply-templates/></span>
</xsl:template>

<xsl:template match='xslthl:attribute'>
  <span class="attribute"><xsl:apply-templates/></span>
</xsl:template>

<xsl:template match='xslthl:value'>
  <span class="value"><xsl:apply-templates/></span>
</xsl:template>

<xsl:template match='xslthl:html'>
  <span class="html"><xsl:apply-templates/></span>
</xsl:template>

<xsl:template match='xslthl:xslt'>
  <span class="xslt"><xsl:apply-templates/></span>
</xsl:template>

</xsl:stylesheet>