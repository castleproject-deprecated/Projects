<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="helper://FormHelper"/>
  <xsl:import href="helper://TestHelper"/>

  <xsl:param name="people"/>

  <xsl:template match="/root">
    <page>
      <title>Home</title>
      <contents>
        <h1>Home page</h1>
        <p>
          <xsl:call-template name="TestHelper-HelloWorld"/>
        </p>
        
        <p>
          <form action ="AddPerson.rails">
            <xsl:text>Please enter your first name: </xsl:text>
            <xsl:call-template name="FormHelper-TextField">
              <xsl:with-param name="target">Person.FirstName</xsl:with-param>
            </xsl:call-template>
            <br/>
            <xsl:text>Please enter your last name: </xsl:text>
            <xsl:call-template name="FormHelper-TextField">
              <xsl:with-param name="target">Person.LastName</xsl:with-param>
            </xsl:call-template>
            <br/>
            <xsl:call-template name="FormHelper-Submit">
              <xsl:with-param name="value">Register</xsl:with-param>
            </xsl:call-template>
          </form>
        </p>
        <p>
          <xsl:text>These are the allready known names:</xsl:text>
          <ul>
            <xsl:apply-templates select="$people"/>
          </ul>
        </p>
      </contents>
    </page>
  </xsl:template>
  <xsl:template match="Person">
    <li>
      <xsl:value-of select="FirstName"/>
      <xsl:text> </xsl:text>
      <xsl:value-of select="LastName"/>
    </li>
  </xsl:template>

</xsl:stylesheet>