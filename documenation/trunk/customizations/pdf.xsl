<?xml version="1.0"?>
<!--
	Generates PDF output.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:import href="../toolchain/docbook-xsl/fo/docbook.xsl" />
	
	<xsl:include href="common.xsl" />
	<xsl:include href="syntax_highlighting.xsl" />

	<!-- Customized HTML stylesheet !-->
	<xsl:param name="html.stylesheet" select="'css/html.css css/syntax_highlighting.css'" />

	<!-- TODO: Check this !-->
	<xsl:param name="fop.extensions" select="1"/>
	
	<xsl:param name="img.src.path" select="'../'" />
  	
	<xsl:param name="section.autolabel" select="1" />
	<xsl:param name="section.autolabel.max.depth">3</xsl:param>
	<xsl:param name="toc.section.depth" select="3"/>
	<xsl:param name="section.label.includes.component.label" select="1"/>
	<xsl:param name="generate.toc" />

</xsl:stylesheet>
