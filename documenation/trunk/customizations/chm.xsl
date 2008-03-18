<?xml version="1.0"?>
<!--
	Generates a CHM Help file.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:import href="../toolchain/docbook-xsl/htmlhelp/htmlhelp.xsl" />
	
	<xsl:include href="common.xsl" />
	<xsl:include href="syntax_highlighting.xsl" />

	<!-- Customized HTML stylesheet !-->
	<xsl:param name="html.stylesheet" select="'css/html.css css/syntax_highlighting.css'" />

	<xsl:param name="suppress.navigation" select="1" />
	<xsl:param name="htmlhelp.hhc.binary" select="0" />
	<xsl:param name="htmlhelp.hhc.folders.instead.books" select="0" />

	<xsl:param name="generate.index" select="1" />
	
	<xsl:param name="chunk.section.depth" select="1" />
	<xsl:param name="generate.toc" />

</xsl:stylesheet>
