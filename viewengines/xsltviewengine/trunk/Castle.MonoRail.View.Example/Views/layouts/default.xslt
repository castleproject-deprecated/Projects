<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
        <title>
          Xslt View Engine Example - <xsl:value-of select="/page/title"/>
        </title>
        <link rel="stylesheet" href="/style.css" />
      </head>
      <body>
        <div id="main">
          <div id="body">
            <xsl:copy-of select="/page/contents/*"/>
          </div>
        </div>
      </body>
    </html>

  </xsl:template>
  
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>