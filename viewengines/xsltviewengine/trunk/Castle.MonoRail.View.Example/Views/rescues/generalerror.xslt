<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
  <xsl:param name="context"/>
  <xsl:template name="ShowException">
    <xsl:param name="exc"/>

    <b>
      <xsl:value-of select="$exc"/>
    </b>


  </xsl:template>

  <xsl:template match="/root">
    <page>
      <title>
        Unexpected error happenend
      </title>
      <contents>
        <h2>
          Unexpected error happenend
        </h2>

        <p> This is the rescue page. See the exception details below </p>

        <pre>
          
          <xsl:call-template name="ShowException">
            <xsl:with-param name="exc">
              <xsl:value-of select="$context/LastException"/>
            </xsl:with-param>
          </xsl:call-template>
        </pre>
      </contents>
    </page>
  </xsl:template>
</xsl:stylesheet>