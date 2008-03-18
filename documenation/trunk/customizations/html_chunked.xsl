<?xml version="1.0"?>
<!--
	Generates multi-file HTML.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:import href="../toolchain/docbook-xsl/html/chunk.xsl" />
	<xsl:include href="html.xsl" />

	<!-- Don't divide chapters in many files !-->
  	<xsl:param name="chunk.section.depth" select="0" />
  	
</xsl:stylesheet>
