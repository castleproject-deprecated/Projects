<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" >
  <!--<xsl:template match="@* | node()">
        <xsl:choose>
         <xsl:when test="@xsi:type">
                     <xsl:element name="{@xsi:type}">
                <xsl:apply-templates select="@* | node()" />
            </xsl:element>
          </xsl:when>
          <xsl:when test="0=1"></xsl:when>
      <xsl:otherwise>
        <xsl:copy>
          <xsl:apply-templates select="@* | node()" />
        </xsl:copy>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>-->

  <xsl:template match="/">
    <xsl:call-template name="test">
      <xsl:with-param name="a">test</xsl:with-param>
      <xsl:with-param name="b">test</xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <xsl:template name="test" >
    <xsl:param name="a"/>
    <xsl:param name="b"/>
    <xsl:param name="c"/>
    <xsl:if test="$a and $b">
      <ok></ok>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>