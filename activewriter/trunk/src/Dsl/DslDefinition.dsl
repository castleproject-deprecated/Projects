<?xml version="1.0" encoding="utf-8"?>
<Dsl dslVersion="1.0.0.0" Id="3a7d8d12-f07c-4dd1-8689-f9610e7a79e1" Description="A Visual Studio 2005 addin to design domain models and to generate code decorated with ActiveRecord attributes based on the model." Name="ActiveWriter" DisplayName="ActiveWriter" Namespace="Altinoren.ActiveWriter" ProductName="ActiveWriter" CompanyName="altinoren.com" PackageGuid="9f4f10d0-81ad-4308-a398-ba7741f3f7e2" PackageNamespace="Altinoren.ActiveWriter" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="fe94e99d-28aa-4a5d-9e43-168adeb72b60" Description="Domain model" Name="Model" DisplayName="Model" Namespace="Altinoren.ActiveWriter">
      <Properties>
        <DomainProperty Id="f85b9035-c755-438f-b54d-2acebc636a8c" Description="Instructs ActiveWriter to generate code using nullables where possible." Name="UseNullables" DisplayName="Use Nullables" DefaultValue="No" Category="Code Generation">
          <Type>
            <DomainEnumerationMoniker Name="NullableUsage" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="571d0e7c-b79b-432e-81ff-2c7665c959b1" Description="Tells ActiveWriter how to name private fields of properties if the Access of Property is PropertyAccess.Property (Default)" Name="CaseOfPrivateFields" DisplayName="Case Of Private Fields" DefaultValue="CamelcaseUnderscore" Category="Code Generation">
          <Type>
            <DomainEnumerationMoniker Name="FieldCase" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="33b55d51-bc5b-4ef3-a07c-8ae6f72531bc" Description="Tells ActiveWriter to generate a seperate MonoRail project with View and Controller code, taking this file's output as the model." Name="GenerateMonoRailProject" DisplayName="Generate MonoRail Project" DefaultValue="false" Category="Code Generation - Web" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2ac42985-50d1-4ba7-bc7a-63432a739578" Description="Name of the MonoRail project. If a project with the same name already exists in the solution, it won't be added to the solution." Name="MonoRailProjectName" DisplayName="MonoRail Project Name" Category="Code Generation - Web" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2e26d6e6-fc84-4453-82b0-1a4599eba39a" Description="Description for Altinoren.ActiveWriter.Model.MonoRail Project Path" Name="MonoRailProjectPath" DisplayName="MonoRail Project Path" Category="Code Generation - Web" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b5eb159d-c09e-4bb5-bbd2-c24a98a187db" Description="" Name="MonoRailDefaultLayout" DisplayName="MonoRail Default Layout" DefaultValue="default" Category="Code Generation - Web" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="32fc8076-7817-4ad8-912d-294fed666072" Description="" Name="MonoRailDefaultRescue" DisplayName="MonoRail Default Rescue" DefaultValue="generalerror" Category="Code Generation - Web" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a2f2a0c7-f6d5-4f0c-8ca3-f7920cc4f546" Description="" Name="MonoRailViewFileExtension" DisplayName="MonoRail View File Extension" DefaultValue=".vm" Category="Code Generation - Web" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="fb8adf70-25ad-408c-8c6b-297e2e719364" Description="Instructs ActiveWriter to generate code using generic collections where possible." Name="UseGenerics" DisplayName="Use Generics" DefaultValue="true" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="bdef414b-e1ca-4d13-9c39-ce0ba00dee7b" Description="Instructs ActiveWriter to generate classes inherited from a base class. If Base Class Name property is empty, ActiveRecordBase or ActiveRecordBase&lt;T&gt; will be used depending on the Use generics property." Name="UseBaseClass" DisplayName="Use Base Class" DefaultValue="true" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ddcc1b4e-07e8-4331-9778-f7cb46375cc1" Description="Defins the base class to be inherited. Does have no effect if Use Base Class property is set to false." Name="BaseClassName" DisplayName="Base Class Name" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c2748bf4-55c5-42b0-8823-6ac381aab13e" Description="If true, will generate a base class with all functionality and a derived partial class to support customization through overrides." Name="GeneratesDoubleDerived" DisplayName="Generates Double Derived" DefaultValue="false" Category="Code Generation" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="98f6b364-1d02-4613-b206-20bd6823b115" Description="Suffix appended to base class names in double derived code generation. Defaults to &quot;Base&quot;, if not supplied." Name="DoubleDerivedNameSuffix" DisplayName="Double Derived Name Suffix" DefaultValue="Base" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="6fe83f5a-b88d-4b40-89b2-ded4bff88a7f" Description="Instructs ActiveWriter to decorate each class with GeneratedCodeAttribute. Some tools evaluate existance of this attribute to handle code differently." Name="UseGeneratedCodeAttribute" DisplayName="Use GeneratedCodeAttribute" DefaultValue="false" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="60bf3b58-876b-4d6a-8e36-21ae054e0daf" Description="Defines the experimental code generation target." Name="Target" DisplayName="Target" DefaultValue="ActiveRecord" Category="Code Generation">
          <Type>
            <DomainEnumerationMoniker Name="CodeGenerationTarget" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c2197ab7-cf01-432f-b44d-ebbb4bdea14d" Description="Provides assembly load path for Castle.ActiveRecord.dll and related assemblies. If required assemblies are in GAC, this setting is omitted. If not and is this setting is blank an error will be thrown during code generation." Name="AssemblyPath" DisplayName="Assembly Path" Category="Code Generation (NHibernate)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="16c1464a-bb58-41bb-ad6b-46b957eec2a2" Description="Provides assembly name to be used during code generation." Name="ActiveRecordAssemblyName" DisplayName="Active Record Assembly Name" DefaultValue="Castle.ActiveRecord, Version=0.0.1.3, Culture=neutral, PublicKeyToken=407dd0808d44fbdc" Category="Code Generation (NHibernate)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a0bcb53c-a856-4a8d-9245-f973dfb023d3" Description="Provides assembly name to be used during code generation." Name="NHibernateAssemblyName" DisplayName="NHibernate Assembly Name" DefaultValue="NHibernate, Version=1.2.0.3001, Culture=neutral, PublicKeyToken=aa95f207798dfdb4" Category="Code Generation (NHibernate)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="3871c9b7-2f36-4c4e-b902-c8848a2842bc" Description="Instructs ActiveWriter to add hbm.xml file as a nested file of this .actiw file. Otherwise, file will be added as a standalone item to the project." Name="RelateWithActiwFile" DisplayName="Relate With Actiw File" DefaultValue="true" Category="Code Generation (NHibernate)">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="652aa8bf-1eb6-47ea-928a-c14b6b950791" Description="Instructs ActiveWriter to generate virtual properties where possible." Name="UseVirtualProperties" DisplayName="Use Virtual Properties" DefaultValue="true" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="88ffcb9e-cd62-45cf-8b96-fbff7f1e1c5f" Description="The namespace to be used in generated code. If not supplied (default), ActiveWriter will use the namespace from the native project structure." Name="Namespace" DisplayName="Namespace" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b0d78aac-cb2a-483c-93c9-b682243a338c" Description="List of additional imports to be added to the generated class." Name="AdditionalImports" DisplayName="Additional Imports" Kind="CustomStorage" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System.Collections.Generic/List&lt;Altinoren.ActiveWriter.Import&gt;" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Notes>Creates an embedding link when an element is dropped onto a model. </Notes>
          <Index>
            <DomainClassMoniker Name="ModelClass" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ModelHasClass.Classes</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="38be3d51-c07f-4b6c-b05a-42f20848e9e2" Description="A class of the domain" Name="ModelClass" DisplayName="Model Class" Namespace="Altinoren.ActiveWriter">
      <BaseClass>
        <DomainClassMoniker Name="ModelElementWithAccess" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="317d9c1a-4f8a-4745-8902-44bc0540e000" Description="" Name="Cache" DisplayName="Cache" DefaultValue="Undefined" Category="ActiveRecord">
          <Type>
            <DomainEnumerationMoniker Name="CacheEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ba3636a4-ae2d-4d3e-9b2a-e93d74b3ac30" Description="Discriminator column for a table inheritance modeling" Name="DiscriminatorColumn" DisplayName="Discriminator Column" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a895f8a6-03ce-4a11-be54-e17e89a6420a" Description="Column type (like string or integer) for the discriminator column" Name="DiscriminatorType" DisplayName="Discriminator Type" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="52115192-5af0-4304-a9aa-1a6925ffb3c1" Description="Value that represents the target class on the discriminator column" Name="DiscriminatorValue" DisplayName="Discriminator Value" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1be0d961-60e7-4ebe-a6a1-37cbdc7e1ac5" Description="Enable lazy loading for the type" Name="Lazy" DisplayName="Lazy" DefaultValue="false" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="743abeab-7821-48a6-9faa-6b74fdc36d17" Description="Associates a proxy type with the target type" Name="Proxy" DisplayName="Proxy" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2a4ece06-b2fc-4bdf-8279-4fa27ae7c6dc" Description="Schema name associated with the type" Name="Schema" DisplayName="Schema" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="10425242-9835-4fab-8b43-98e51035d3df" Description="Table name associated with the type" Name="Table" DisplayName="Table" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="128b9af7-5c8d-4e69-b27b-d2a5b658a618" Description="SQL condition to retrieve objects" Name="Where" DisplayName="Where" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="834cd223-036c-49f8-9830-617b34ef00d4" Description="Description for Altinoren.ActiveWriter.ModelClass.Has Key Property" Name="HasKeyProperty" DisplayName="Has Key Property" Kind="Calculated" SetterAccessModifier="Private" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="00ceff38-d06d-4eeb-8a77-4de5abfc4880" Description="Description for Altinoren.ActiveWriter.ModelClass.Is Validator Set" Name="IsValidatorSet" DisplayName="Is Validator Set" Kind="Calculated" SetterAccessModifier="Private" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7bfa1887-17b0-45c6-bd09-a9aaae89f060" Description="Specifies that INSERT SQL should be generated at runtime and contains only the columns whose values are not null." Name="DynamicInsert" DisplayName="Dynamic Insert" DefaultValue="false" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2de755fe-84c3-4ee3-958a-81ca601ad7d8" Description="Specifies that UPDATE SQL should be generated at runtime and contains only those columns whose values have changed." Name="DynamicUpdate" DisplayName="Dynamic Update" DefaultValue="false" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5b5a2aba-a22e-40b3-a8e2-253ce3e87be0" Description="Specifies a custom IEntityPersister." Name="Persister" DisplayName="Persister" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="68ae3759-c4b1-4d71-a60b-0f022906d020" Description="Specifies that NHibernate should never perform an SQL UPDATE unless it is certain that an  object is actually modified. In certain cases (actually, only when a transient object has been associated with a new session using update()), this means that NHibernate will perform an extra SQL SELECT to determine if an UPDATE is actually required." Name="SelectBeforeUpdate" DisplayName="Select Before Update" DefaultValue="false" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5e2047eb-e145-40ca-8380-ac0234e31719" Description="Determines whether implicit or explicit query polymorphism is used." Name="Polymorphism" DisplayName="Polymorphism" DefaultValue="Implicit" Category="ActiveRecord">
          <Type>
            <DomainEnumerationMoniker Name="Polymorphism" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="35f70fd2-3f8e-48fa-9e0b-b09239aa0953" Description="Specifies that instances of the class are (not) mutable." Name="Mutable" DisplayName="Mutable" DefaultValue="true" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7b8bf339-10c5-46f8-96bc-bb893d7a404d" Description="Specify a &quot;batch size&quot; for fetching instances of this class by identifier." Name="BatchSize" DisplayName="Batch Size" DefaultValue="1" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e4844c20-3d13-4a57-85e2-4049ed5fae88" Description="Description for Altinoren.ActiveWriter.ModelClass.Locking" Name="Locking" DisplayName="Locking" DefaultValue="Version" Category="ActiveRecord">
          <Type>
            <DomainEnumerationMoniker Name="OptimisticLocking" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="bf44f952-fe91-4611-953a-ddcaeae9458e" Description="The auto-import attribute lets us use unqualified class names in the query language, by default. The assembly and namespace attributes specify the assembly where persistent classes are located and the namespace they are declared in." Name="UseAutoImport" DisplayName="Use Auto Import" DefaultValue="true" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ModelProperty" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ClassHasProperty.Properties</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="03f30fca-953c-4ab2-b3e4-956a355e0768" Description="A property or field of a class" Name="ModelProperty" DisplayName="Property" Namespace="Altinoren.ActiveWriter">
      <BaseClass>
        <DomainClassMoniker Name="ModelElementWithAccess" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="37bf5f3f-4a5b-4b1d-802b-a6401324f13f" Description="" Name="Column" DisplayName="Column" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e0dd7a3b-0ec5-4b7e-9e50-44141167b5ae" Description="" Name="ColumnType" DisplayName="Column Type" DefaultValue="String" Category="ActiveRecord">
          <Type>
            <DomainEnumerationMoniker Name="NHibernateType" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5389a767-406a-408c-9b0c-877248abb696" Description="" Name="Formula" DisplayName="Formula" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="667f32aa-4de5-4f88-9e3c-a9f3e5190821" Description="" Name="Insert" DisplayName="Insert" DefaultValue="true" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c34e5c57-2cee-46ca-b559-91a773811939" Description="" Name="Length" DisplayName="Length" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ab4fe9fd-d71b-4aec-981e-7631bd7c9b06" Description="" Name="NotNull" DisplayName="Not Null" DefaultValue="false" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="29082743-d810-4131-88a6-ed6b7ec209ce" Description="" Name="Unique" DisplayName="Unique" DefaultValue="false" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="85b4e54d-4986-463a-a7e2-b15758af79aa" Description="" Name="UnsavedValue" DisplayName="Unsaved Value" Category="ActiveRecord Primary Key">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f404d3e6-4cd2-48e0-bb8f-1780865244a2" Description="" Name="Update" DisplayName="Update" DefaultValue="true" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="fab33357-7061-42bc-b5a5-5238cb8ae4a9" Description="" Name="Generator" DisplayName="Generator" DefaultValue="Native" Category="ActiveRecord Primary Key">
          <Type>
            <DomainEnumerationMoniker Name="PrimaryKeyType" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="04023409-cb73-4f64-ae64-3e1fbbcb0ec1" Description="Denotes if the property is a normal property, a key or part of of a primary key." Name="KeyType" DisplayName="Key Type" DefaultValue="None" Category="ActiveRecord Primary Key">
          <Type>
            <DomainEnumerationMoniker Name="KeyType" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1a3c7764-db0d-4bcf-bf5d-dde0ada98f3d" Description="" Name="Params" DisplayName="Params" Category="ActiveRecord Primary Key">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="0fd9ef70-8f0b-4d4d-bf84-d0df062d763a" Description="Name of the sequence to be used for the primary key. Required when Generator is Sequence." Name="SequenceName" DisplayName="Sequence Name" Category="ActiveRecord Primary Key">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2f3d7c02-f986-4d98-9e1d-fadfa521dbd2" Description="Accessor of the property." Name="Accessor" DisplayName="Accessor" DefaultValue="Public" Category="Code Generation">
          <Type>
            <DomainEnumerationMoniker Name="Accessor" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="74868328-9484-406e-94be-d6a85d8a28c7" Description="Name of the composite key class to be generated." Name="CompositeKeyName" DisplayName="Composite Key Name" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="4ed3c7b2-58e8-4db7-be1d-bf46273a3756" Description="Type of the property." Name="PropertyType" DisplayName="Property Type" DefaultValue="Property" Category="ActiveRecord">
          <Type>
            <DomainEnumerationMoniker Name="PropertyType" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b70d57e3-356e-4abb-bcc1-d0f7386ad74a" Description="Decorates the class this property belongs to with DebuggerDisplay attribute, using this property as the display source. Example:  [DebuggerDisplay(&quot;Name = {Name}&quot;)]" Name="DebuggerDisplay" DisplayName="Debugger Display" DefaultValue="false" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="52415453-1653-41e9-a3f6-7792634f3981" Description="Defines validators to be used on the property." Name="Validator" DisplayName="Validator" Kind="CustomStorage" Category="ActiveRecord">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Altinoren.ActiveWriter.UIEditors.PropertyValidationEditor), typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="4044711f-d9ec-4e57-bf04-adae64ce1800" Description="A unique-key attribute can be used to group columns in a single unit key constraint." Name="UniqueKey" DisplayName="Unique Key" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="485e004e-dd51-4770-8d09-774b44d9e25b" Description="Specifies the name of a (multi-column) index." Name="Index" DisplayName="Index" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ca48b1f1-fbf3-4a32-b9d5-4dd10b962079" Description="Overrides the default column type." Name="SqlType" DisplayName="Sql Type" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="54fa865c-a419-478c-a46d-c1085e92a2b2" Description="Create an SQL check constraint on either column or table." Name="Check" DisplayName="Check" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="cbea293a-1057-4c5b-b60d-78728f734ef2" Description="Description for Altinoren.ActiveWriter.ModelElementWithAccess" Name="ModelElementWithAccess" DisplayName="Model Element With Access" InheritanceModifier="Abstract" Namespace="Altinoren.ActiveWriter">
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="86160079-d659-41e2-8718-f83de10ddb88" Description="" Name="CustomAccess" DisplayName="Custom Access" Category="ActiveRecord">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="64a542a8-a0ce-42cc-b9a8-1845c744cb97" Description="" Name="Access" DisplayName="Access" DefaultValue="Property" Category="ActiveRecord">
          <Type>
            <DomainEnumerationMoniker Name="PropertyAccess" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="ab60f55a-7fb1-44a9-9832-f708caeeee43" Description="Description for Altinoren.ActiveWriter.NamedElement" Name="NamedElement" DisplayName="Named Element" InheritanceModifier="Abstract" Namespace="Altinoren.ActiveWriter">
      <Properties>
        <DomainProperty Id="171d6e97-362e-4ae7-a2a7-9d1814709b1f" Description="Name of the element" Name="Name" DisplayName="Name" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7e532084-1f9d-4d7a-945c-cc7b484839d3" Description="Description of the element. This will appear in a &lt;summary&gt; Xml comment." Name="Description" DisplayName="Description">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="f9419603-bb7b-40fc-9fea-fcd2e7e1b990" Description="Description for Altinoren.ActiveWriter.ModelHasClass" Name="ModelHasClass" DisplayName="Model Has Class" Namespace="Altinoren.ActiveWriter" IsEmbedding="true">
      <Source>
        <DomainRole Id="9e22f600-80ad-4883-a32f-df2738ee3614" Description="Classes of the model" Name="Model" DisplayName="Model" PropertyName="Classes" PropertyDisplayName="Classes">
          <RolePlayer>
            <DomainClassMoniker Name="Model" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="91a1db06-cbe8-44b8-a827-329a2ca102c3" Description="Model of the class" Name="Class" DisplayName="Class" PropertyName="Model" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Model">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="68ba5474-0696-4988-9e76-1bc8e9eb9de8" Description="Maps a many to one association" Name="ManyToOneRelation" DisplayName="Many To One Relation" Namespace="Altinoren.ActiveWriter" AllowsDuplicates="true">
      <Properties>
        <DomainProperty Id="c3ac9ce5-87b4-48ca-a243-26e0a237e7ab" Description="" Name="TargetCache" DisplayName="Target Cache" DefaultValue="Undefined" Category="Target (HasMany)">
          <Type>
            <DomainEnumerationMoniker Name="CacheEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="55f2f2e3-3183-4b1a-b3c9-9a864fb2e4ae" Description="" Name="TargetCascade" DisplayName="Target Cascade" DefaultValue="None" Category="Target (HasMany)">
          <Type>
            <DomainEnumerationMoniker Name="ManyRelationCascadeEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e4d0bfec-46a2-466d-b8f2-5545ebb18658" Description="" Name="TargetColumnKey" DisplayName="Target Column Key" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="76ddf835-fca5-44dc-abb1-adc0c1563db4" Description="" Name="TargetCustomAccess" DisplayName="Target Custom Access" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="396220b5-9996-4ca6-9cbb-fca174540765" Description="" Name="TargetInverse" DisplayName="Target Inverse" DefaultValue="false" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b99bdbe1-4c3b-4040-aa1e-4f0f1f081b93" Description="" Name="TargetLazy" DisplayName="Target Lazy" DefaultValue="false" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1a4f7b45-a9e0-4489-a71e-f2a8aecbfb7f" Description="" Name="TargetMapType" DisplayName="Target Map Type" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ec6503aa-f498-4992-bd6f-baa9fcc568b3" Description="" Name="TargetOrderBy" DisplayName="Target Order By" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="8c4ed47e-37dc-4975-b996-7c6521940bf2" Description="" Name="TargetRelationType" DisplayName="Target Relation Type" DefaultValue="Guess" Category="Target (HasMany)">
          <Type>
            <DomainEnumerationMoniker Name="RelationType" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="3da30e35-2a3d-4e5e-ab57-8f71af79c4c4" Description="" Name="TargetSchema" DisplayName="Target Schema" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="aa0eab75-566c-40a8-ae2c-ddf13effa636" Description="" Name="TargetSort" DisplayName="Target Sort" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="dbace6bc-2b15-4b78-a57d-95852b8cd583" Description="" Name="TargetTable" DisplayName="Target Table" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2f1e9945-4335-4316-a705-3e27a0943b61" Description="" Name="TargetWhere" DisplayName="Target Where" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e4a0ff32-b309-439a-94b5-de8d3ef7d0ca" Description="Description of the property. This will appear in a &lt;summary&gt; Xml comment." Name="TargetDescription" DisplayName="Target Description" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="658c3768-1a8c-442c-8ebe-df02007cc3a1" Description="Name of the property in the generated code. If not supplied, ActiveWriter will use the source class name in plural form." Name="TargetPropertyName" DisplayName="Target Property Name" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f415a978-5c96-46e1-900e-443303aa5a37" Description="Type of the property. If not supplied, ActiveWriter will use IList by default." Name="TargetPropertyType" DisplayName="Target Property Type" DefaultValue="IList" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="563916db-4a01-4bd6-b6b4-aab15e396f78" Description="" Name="SourceCascade" DisplayName="Source Cascade" DefaultValue="None" Category="Source (BelongsTo)">
          <Type>
            <DomainEnumerationMoniker Name="CascadeEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="78d006af-6e94-463b-a5bf-fba65534f272" Description="" Name="SourceColumn" DisplayName="Source Column" Category="Source (BelongsTo)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f24290b4-9a78-4bd0-b9ba-4f7973768389" Description="" Name="SourceCustomAccess" DisplayName="Source Custom Access" Category="Source (BelongsTo)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="69f188c5-6e65-4d3c-ad92-5f706d79eaad" Description="" Name="SourceInsert" DisplayName="Source Insert" DefaultValue="true" Category="Source (BelongsTo)">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="12d3226e-3185-44be-af32-1e3459cad8e9" Description="" Name="SourceNotNull" DisplayName="Source Not Null" DefaultValue="false" Category="Source (BelongsTo)">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d5bd0e29-9032-4775-9711-df5b24f9914d" Description="" Name="SourceOuterJoin" DisplayName="Source Outer Join" DefaultValue="Auto" Category="Source (BelongsTo)">
          <Type>
            <DomainEnumerationMoniker Name="OuterJoinEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d03c165e-b655-4428-a368-6a9184805470" Description="" Name="SourceType" DisplayName="Source Type" Category="Source (BelongsTo)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c2e055f1-5410-4488-90cd-278a8a494ab7" Description="" Name="SourceUnique" DisplayName="Source Unique" DefaultValue="false" Category="Source (BelongsTo)">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="328a72d1-69d3-45f5-b671-27b0b8ce08f1" Description="" Name="SourceUpdate" DisplayName="Source Update" DefaultValue="true" Category="Source (BelongsTo)">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="4090d713-654c-48c3-9640-73ad00dbc4b6" Description="Description of the property. This will appear in a &lt;summary&gt; Xml comment." Name="SourceDescription" DisplayName="Source Description" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="05776d07-5fe2-4041-84fc-8d0c1d3b21ec" Description="The access strategy." Name="TargetAccess" DisplayName="Target Access" DefaultValue="Property" Category="Target (HasMany)">
          <Type>
            <DomainEnumerationMoniker Name="PropertyAccess" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="46603735-0a4c-4811-a643-125844df7d70" Description="Name of the property. If not suplied, ActiveWriter will use target class name." Name="SourcePropertyName" DisplayName="Source Property Name" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e9556820-fa84-4921-97a3-c7cc3b45d539" Description="Gets or sets the way broken relations are handled." Name="SourceNotFoundBehaviour" DisplayName="Source Not Found Behaviour" DefaultValue="Default" Category="Source (BelongsTo)">
          <Type>
            <DomainEnumerationMoniker Name="NotFoundBehaviour" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="91224e05-42d5-4913-9659-aa8996fb8240" Description="Gets or sets the way broken relations are handled." Name="TargetNotFoundBehaviour" DisplayName="Target Not Found Behaviour" DefaultValue="Default" Category="Target (HasMany)">
          <Type>
            <DomainEnumerationMoniker Name="NotFoundBehaviour" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="0c3d3bc5-bea7-4146-8a8e-943626595d5b" Description="Use for simple types." Name="TargetElement" DisplayName="Target Element" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="90d3139c-f9b1-4dd1-8db6-7c8564abb213" Description="Only used with maps." Name="TargetIndexType" DisplayName="Target Index Type" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="efe49545-0679-4e40-967e-e8034c83f70e" Description="Only used with maps or list." Name="TargetIndex" DisplayName="Target Index" Category="Target (HasMany)">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="8fc70677-e363-4cda-b821-17505cab2c14" Description="" Name="Source" DisplayName="Source" PropertyName="Targets" PropertyDisplayName="Targets">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="25d422d4-6342-4127-9b72-33da88df08be" Description="" Name="Target" DisplayName="Target" PropertyName="Sources" PropertyDisplayName="Sources">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="8f270f87-f8f4-4f43-9fe5-13b75748144c" Description="" Name="ClassHasProperty" DisplayName="Class Has Property" Namespace="Altinoren.ActiveWriter" IsEmbedding="true">
      <Source>
        <DomainRole Id="8eddf27d-b9d1-4813-940e-c374248112cb" Description="" Name="ModelClass" DisplayName="Model Class" PropertyName="Properties" PropertyDisplayName="Properties">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="2a4fd4f7-6c32-4821-9f28-f596ad9423f0" Description="" Name="Property" DisplayName="Property" PropertyName="ModelClass" Multiplicity="ZeroOne" PropertyDisplayName="Model Class">
          <RolePlayer>
            <DomainClassMoniker Name="ModelProperty" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="3facccc2-b82b-4dd9-b414-1a787f87ecd8" Description="Maps a many to many association with an association table" Name="ManyToManyRelation" DisplayName="Many To Many Relation" Namespace="Altinoren.ActiveWriter" AllowsDuplicates="true">
      <Properties>
        <DomainProperty Id="0edaa292-6e4e-403e-87f5-c6e3b65d37c7" Description="" Name="SourceCache" DisplayName="Source Cache" DefaultValue="Undefined" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="CacheEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b1eb524a-89f0-407d-a7d2-fe54e92c091e" Description="" Name="SourceCascade" DisplayName="Source Cascade" DefaultValue="None" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="ManyRelationCascadeEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="45f8bd67-d220-4187-a3c9-0cf6bab3057f" Description="" Name="SourceColumn" DisplayName="Source Column" Category="Common">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="3ccebabd-ae79-43a0-bb64-65c687801773" Description="" Name="SourceCustomAccess" DisplayName="Source Custom Access" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1f969dd1-34d3-45f0-9c0b-997e466e51ac" Description="" Name="SourceInverse" DisplayName="Source Inverse" DefaultValue="false" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="8243d2da-48fc-4c4e-b767-54761c5d8ecf" Description="" Name="SourceLazy" DisplayName="Source Lazy" DefaultValue="false" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1bfebb0e-ef50-4f1d-8d31-fc6cd8305f6c" Description="" Name="SourceMapType" DisplayName="Source Map Type" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="36f64f63-e9e8-4baf-bd34-93a6a714ad9f" Description="" Name="SourceOrderBy" DisplayName="Source Order By" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1a7ce077-b01a-4e14-ba2e-81f24d1645ac" Description="" Name="SourceRelationType" DisplayName="Source Relation Type" DefaultValue="Guess" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="RelationType" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="aa9b2d6f-225f-42c5-8871-05df449fc598" Description="" Name="Schema" DisplayName="Schema" Category="Common">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="8f6bcfce-156a-4224-bac8-8ef0f973a56a" Description="" Name="SourceSort" DisplayName="Source Sort" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c0b31c9a-a436-4cc8-bf42-c29f75be6bc5" Description="" Name="Table" DisplayName="Table" Category="Common">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="df150009-69a6-4cc6-b6a4-96beabc88c85" Description="" Name="SourceWhere" DisplayName="Source Where" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e7614cb3-b408-4170-91bd-0d8fa3213be9" Description="" Name="TargetCache" DisplayName="Target Cache" DefaultValue="Undefined" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="CacheEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="79db5e78-57ca-41f6-b409-f54c3692c71b" Description="" Name="TargetCascade" DisplayName="Target Cascade" DefaultValue="None" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="ManyRelationCascadeEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="6b974a6d-7e61-4920-8ced-490f4a59f590" Description="" Name="TargetColumn" DisplayName="Target Column" Category="Common">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="3f6c6de8-21b0-4c86-9f39-5f0b4b3ff24d" Description="" Name="TargetCustomAccess" DisplayName="Target Custom Access" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="50027d75-155d-481d-9074-e15c47a42df8" Description="" Name="TargetInverse" DisplayName="Target Inverse" DefaultValue="false" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e9403ad1-f969-4640-8422-ef4ba7d1b34a" Description="" Name="TargetLazy" DisplayName="Target Lazy" DefaultValue="false" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="4b222ca6-b4c7-42b7-a357-60c3dc0256b1" Description="" Name="TargetMapType" DisplayName="Target Map Type" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="490b0adb-fb17-44af-8852-00f62231b389" Description="" Name="TargetOrderBy" DisplayName="Target Order By" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="913caa46-b08b-45bb-ac6f-6ef394c59938" Description="" Name="TargetRelationType" DisplayName="Target Relation Type" DefaultValue="Guess" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="RelationType" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="65664bb5-5639-4405-85d0-837c8c56e1b8" Description="" Name="TargetSort" DisplayName="Target Sort" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f1a74b56-59f7-4dad-aac8-da5eb7f3fd15" Description="" Name="TargetWhere" DisplayName="Target Where" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d8741eea-178b-405f-8159-76e839d77970" Description="Description of the property. This will appear in a &lt;summary&gt; Xml comment." Name="SourceDescription" DisplayName="Source Description" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7286c00d-568a-41d1-8d53-fe673520aeb9" Description="Description of the property. This will appear in a &lt;summary&gt; Xml comment." Name="TargetDescription" DisplayName="Target Description" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="9e857994-04c2-40f0-99b1-d909623a6873" Description="" Name="SourceAccess" DisplayName="Source Access" DefaultValue="Property" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="PropertyAccess" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="308436c6-0b6c-4887-b5d4-e15c9174f9cc" Description="" Name="TargetAccess" DisplayName="Target Access" DefaultValue="Property" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="PropertyAccess" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e5d28e22-22c2-48bb-9540-8be851751818" Description="Type of the property. If not supplied, ActiveWriter will use IList by default." Name="SourcePropertyType" DisplayName="Source Property Type" DefaultValue="IList" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="9a519de5-5dbd-4c8b-91ab-7c2ba677566a" Description="Type of the property. If not supplied, ActiveWriter will use IList by default." Name="TargetPropertyType" DisplayName="Target Property Type" DefaultValue="IList" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="894becde-cb4f-4c73-bcf8-39a2cd831096" Description="Name of the property in the generated code. If not supplied, ActiveWriter will use the target class name in plural form." Name="SourcePropertyName" DisplayName="Source Property Name" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="486cb585-d891-4eb2-97de-846aeaec33e5" Description="Name of the property in the generated code. If not supplied, ActiveWriter will use the source class name in plural form." Name="TargetPropertyName" DisplayName="Target Property Name" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="29123760-24b6-42d1-a9b1-fb8b1d08d198" Description="Gets or sets the way broken relations are handled." Name="TargetNotFoundBehaviour" DisplayName="Target Not Found Behaviour" DefaultValue="Default" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="NotFoundBehaviour" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="32c73c34-5d19-4c1c-8427-91646d05fb53" Description="Gets or sets the way broken relations are handled." Name="SourceNotFoundBehaviour" DisplayName="Source Not Found Behaviour" DefaultValue="Default" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="NotFoundBehaviour" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="a09915cc-6063-4ccb-b932-1dc8660fc47c" Description="" Name="Source" DisplayName="Source" PropertyName="ManyToManyTargets" PropertyDisplayName="Many To Many Targets">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="2cf13e85-7739-4764-9d1a-f68680e977be" Description="" Name="Target" DisplayName="Target" PropertyName="ManyToManySources" PropertyDisplayName="Many To Many Sources">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="fc147ce4-8aa0-4719-94a9-21179d6cf6a5" Description="Associates a foreign table where the current class and the target class share their primary key." Name="OneToOneRelation" DisplayName="One To One Relation" Namespace="Altinoren.ActiveWriter">
      <Properties>
        <DomainProperty Id="6cab92a3-03e6-41fa-926d-f33afdbbbfcd" Description="" Name="SourceAccess" DisplayName="Source Access" DefaultValue="Property" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="PropertyAccess" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1d99e2db-2ed5-42dc-b869-cfb8041d97b5" Description="" Name="SourceCascade" DisplayName="Source Cascade" DefaultValue="None" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="CascadeEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="9266b2c1-d18e-41ae-818e-baff98d89724" Description="" Name="SourceConstrained" DisplayName="Source Constrained" DefaultValue="false" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b3b9f301-bbbb-4a4e-a9bc-31511ba03ab0" Description="" Name="SourceCustomAccess" DisplayName="Source Custom Access" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5d4c0adf-fbb4-41b1-b615-6e6690cc20c6" Description="" Name="SourceOuterJoin" DisplayName="Source Outer Join" DefaultValue="Auto" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="OuterJoinEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f51a55ef-21f7-4e22-9776-14c00caf8879" Description="" Name="TargetAccess" DisplayName="Target Access" DefaultValue="Property" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="PropertyAccess" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="42102a9d-77ce-437e-abc7-3cd54e8a1061" Description="" Name="TargetCascade" DisplayName="Target Cascade" DefaultValue="None" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="CascadeEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c38a4744-ad38-4be2-acc9-1a32330fbed0" Description="" Name="TargetConstrained" DisplayName="Target Constrained" DefaultValue="false" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="71644b41-9c91-40ad-b8f7-83c91d8239cb" Description="" Name="TargetCustomAccess" DisplayName="Target Custom Access" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="51177b34-f743-4cb8-860f-d0d7b6b00ba6" Description="" Name="TargetOuterJoin" DisplayName="Target Outer Join" DefaultValue="Auto" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="OuterJoinEnum" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="892bd79a-5ea9-44e9-bffa-bde7c18c9560" Description="Description of the property. This will appear in a &lt;summary&gt; Xml comment." Name="SourceDescription" DisplayName="Source Description" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="742dacbc-a8a5-41d1-9f4a-b5c370057902" Description="Description of the property. This will appear in a &lt;summary&gt; Xml comment." Name="TargetDescription" DisplayName="Target Description" Category="Code Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="7b304a5c-cb3e-4f10-be86-c52eeec945d5" Description="" Name="Source" DisplayName="Source" PropertyName="OneToOneTarget" Multiplicity="ZeroOne" IsPropertyBrowsable="false" PropertyDisplayName="One To One Target">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="24658a8c-da41-491a-8cad-9c5125cdd06c" Description="" Name="Target" DisplayName="Target" PropertyName="OneToOneSources" PropertyDisplayName="One To One Sources">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
  </Relationships>
  <Types>
    <ExternalType Name="DateTime" Namespace="System" />
    <ExternalType Name="String" Namespace="System" />
    <ExternalType Name="Int16" Namespace="System" />
    <ExternalType Name="Int32" Namespace="System" />
    <ExternalType Name="Int64" Namespace="System" />
    <ExternalType Name="UInt16" Namespace="System" />
    <ExternalType Name="UInt32" Namespace="System" />
    <ExternalType Name="UInt64" Namespace="System" />
    <ExternalType Name="SByte" Namespace="System" />
    <ExternalType Name="Byte" Namespace="System" />
    <ExternalType Name="Double" Namespace="System" />
    <ExternalType Name="Single" Namespace="System" />
    <ExternalType Name="Guid" Namespace="System" />
    <ExternalType Name="Boolean" Namespace="System" />
    <ExternalType Name="Char" Namespace="System" />
    <DomainEnumeration Name="PropertyAccess" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.PropertyAccess">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.Property" Name="Property" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.FieldCamelcase" Name="FieldCamelcase" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.FieldCamelcaseUnderscore" Name="FieldCamelcaseUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.FieldPascalcaseMUnderscore" Name="FieldPascalcaseMUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.FieldLowercaseUnderscore" Name="FieldLowercaseUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.NosetterCamelcase" Name="NosetterCamelcase" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.NosetterCamelcaseUnderscore" Name="NosetterCamelcaseUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.NosetterPascalcaseMUnderscore" Name="NosetterPascalcaseMUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.NosetterLowercaseUnderscore" Name="NosetterLowercaseUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.Field" Name="Field" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyAccess.NosetterLowercase" Name="NosetterLowercase" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="CacheEnum" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.CacheEnum">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CacheEnum.Undefined" Name="Undefined" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CacheEnum.ReadOnly" Name="ReadOnly" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CacheEnum.ReadWrite" Name="ReadWrite" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CacheEnum.NonStrictReadWrite" Name="NonStrictReadWrite" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="PrimaryKeyType" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.PrimaryKeyType">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.None" Name="None" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.Identity" Name="Identity" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.Sequence" Name="Sequence" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.HiLo" Name="HiLo" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.SeqHiLo" Name="SeqHiLo" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.UuidHex" Name="UuidHex" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.UuidString" Name="UuidString" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.Guid" Name="Guid" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.GuidComb" Name="GuidComb" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.Native" Name="Native" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.Assigned" Name="Assigned" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PrimaryKeyType.Foreign" Name="Foreign" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="KeyType" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.KeyType">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.KeyType.None" Name="None" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.KeyType.PrimaryKey" Name="PrimaryKey" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.KeyType.CompositeKey" Name="CompositeKey" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="CascadeEnum" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.CascadeEnum">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CascadeEnum.None" Name="None" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CascadeEnum.All" Name="All" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CascadeEnum.SaveUpdate" Name="SaveUpdate" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CascadeEnum.Delete" Name="Delete" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="OuterJoinEnum" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.OuterJoinEnum">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.OuterJoinEnum.Auto" Name="Auto" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.OuterJoinEnum.True" Name="True" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.OuterJoinEnum.False" Name="False" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="RelationType" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.RelationType">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.RelationType.Guess" Name="Guess" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.RelationType.Bag" Name="Bag" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.RelationType.Set" Name="Set" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.RelationType.IdBag" Name="IdBag" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.RelationType.Map" Name="Map" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="NHibernateType" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.NHibernateType">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.AnsiChar" Name="AnsiChar" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Single" Name="Single" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.AnsiString" Name="AnsiString" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.CultureInfo" Name="CultureInfo" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Binary" Name="Binary" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Type" Name="Type" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.String" Name="String" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.StringClob" Name="StringClob" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.BinaryBlob" Name="BinaryBlob" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Boolean" Name="Boolean" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Byte" Name="Byte" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Char" Name="Char" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.DateTime" Name="DateTime" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Decimal" Name="Decimal" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Double" Name="Double" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Guid" Name="Guid" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Int16" Name="Int16" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Int32" Name="Int32" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Int64" Name="Int64" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Ticks" Name="Ticks" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.TimeSpan" Name="TimeSpan" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.Timestamp" Name="Timestamp" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.TrueFalse" Name="TrueFalse" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NHibernateType.YesNo" Name="YesNo" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="Accessor" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.Accessor">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.Accessor.Public" Name="Public" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.Accessor.Private" Name="Private" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.Accessor.Protected" Name="Protected" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="PropertyType" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.PropertyType">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyType.Property" Name="Property" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyType.Field" Name="Field" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyType.Version" Name="Version" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.PropertyType.Timestamp" Name="Timestamp" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="FieldCase" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.FieldCase">
      <Literals>
        <EnumerationLiteral Description="" Name="Unchanged" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.FieldCase.Camelcase" Name="Camelcase" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.FieldCase.CamelcaseUnderscore" Name="CamelcaseUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.FieldCase.CamelcaseMUnderscore" Name="CamelcaseMUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.FieldCase.Pascalcase" Name="Pascalcase" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.FieldCase.PascalcaseUnderscore" Name="PascalcaseUnderscore" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.FieldCase.PascalcaseMUnderscore" Name="PascalcaseMUnderscore" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="CodeLanguage" Namespace="Altinoren.ActiveWriter" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="CSharp" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CodeLanguage.VB" Name="VB" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="NullableUsage" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.NullableUsage">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NullableUsage.No" Name="No" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NullableUsage.Native" Name="Native" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NullableUsage.WithHelperLibrary" Name="WithHelperLibrary" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="Polymorphism" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.Polymorphism">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.Polymorphism.Implicit" Name="Implicit" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.Polymorphism.Explicit" Name="Explicit" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="OptimisticLocking" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.OptimisticLocking">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.OptimisticLocking.None" Name="None" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.OptimisticLocking.Version" Name="Version" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.OptimisticLocking.Dirty" Name="Dirty" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.OptimisticLocking.All" Name="All" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="NotFoundBehaviour" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.NotFoundBehaviour">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NotFoundBehaviour.Default" Name="Default" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NotFoundBehaviour.Exception" Name="Exception" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.NotFoundBehaviour.Ignore" Name="Ignore" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="CodeGenerationTarget" Namespace="Altinoren.ActiveWriter" Description="">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.CodeGenerationTarget.ActiveRecord" Name="ActiveRecord" Value="" />
        <EnumerationLiteral Description="" Name="NHibernate" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="ManyRelationCascadeEnum" Namespace="Altinoren.ActiveWriter" Description="Description for Altinoren.ActiveWriter.ManyRelationCascadeEnum">
      <Literals>
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.ManyRelationCascadeEnum.None" Name="None" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.ManyRelationCascadeEnum.All" Name="All" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.ManyRelationCascadeEnum.SaveUpdate" Name="SaveUpdate" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.ManyRelationCascadeEnum.Delete" Name="Delete" Value="" />
        <EnumerationLiteral Description="Description for Altinoren.ActiveWriter.ManyRelationCascadeEnum.AllDeleteOrphan" Name="AllDeleteOrphan" Value="" />
      </Literals>
    </DomainEnumeration>
    <ExternalType Name="List&lt;Altinoren.ActiveWriter.Import&gt;" Namespace="System.Collections.Generic" />
    <ExternalType Name="Array" Namespace="System" />
  </Types>
  <Shapes>
    <CompartmentShape Id="ca45d586-12d1-4f5d-99c7-83c1eb0e61eb" Description="" Name="ClassShape" DisplayName="Class Shape" Namespace="Altinoren.ActiveWriter" GeneratesDoubleDerived="true" FixedTooltipText="Class Shape" FillColor="LightSteelBlue" InitialHeight="0.4" Geometry="RoundedRectangle">
      <ShapeHasDecorators Position="InnerTopCenter" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="Name" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapse" DisplayName="Expand Collapse" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0">
        <IconDecorator Name="Key" DisplayName="Key" DefaultIcon="Resources\key.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0.14">
        <IconDecorator Name="Validation" DisplayName="Validation" DefaultIcon="Resources\validation.png" />
      </ShapeHasDecorators>
      <Compartment Name="Properties" Title="Properties" />
    </CompartmentShape>
  </Shapes>
  <Connectors>
    <Connector Id="e7702e83-2753-42c9-85ce-3a19239d6012" Description="Description for Altinoren.ActiveWriter.ManyToOneConnector" Name="ManyToOneConnector" DisplayName="Many To One Connector" Namespace="Altinoren.ActiveWriter" FixedTooltipText="Many To One Connector" SourceEndStyle="EmptyDiamond" TargetEndStyle="EmptyArrow" Thickness="0.01">
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="One" DisplayName="One" DefaultText="1" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="SourceTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="Many" DisplayName="Many" DefaultText="n" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="2c40f7b4-090f-4cc2-802e-1aa792bf4785" Description="Description for Altinoren.ActiveWriter.ManyToManyConnector" Name="ManyToManyConnector" DisplayName="Many To Many Connector" Namespace="Altinoren.ActiveWriter" FixedTooltipText="Many To Many Connector" SourceEndStyle="EmptyDiamond" TargetEndStyle="EmptyDiamond" Thickness="0.01">
      <ConnectorHasDecorators Position="SourceTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="ManySource" DisplayName="Many Source" DefaultText="n" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="ManyTarget" DisplayName="Many Target" DefaultText="m" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="be1643a1-a849-4031-9f25-eff913be9cce" Description="Associates a foreign table where the current class and the target class share their primary key." Name="OneToOneConnector" DisplayName="One To One Connector" Namespace="Altinoren.ActiveWriter" FixedTooltipText="One To One Connector" SourceEndStyle="EmptyArrow" TargetEndStyle="FilledArrow" Thickness="0.01">
      <ConnectorHasDecorators Position="SourceTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="OneSource" DisplayName="One Source" DefaultText="1" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="OneTarget" DisplayName="One Target" DefaultText="1" />
      </ConnectorHasDecorators>
    </Connector>
  </Connectors>
  <XmlSerializationBehavior Name="ActiveWriterSerializationBehavior" Namespace="Altinoren.ActiveWriter">
    <ClassData>
      <XmlClassData TypeName="Model" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelMoniker" ElementName="model" MonikerTypeName="ModelMoniker">
        <DomainClassMoniker Name="Model" />
        <ElementData>
          <XmlRelationshipData RoleElementName="classes">
            <DomainRelationshipMoniker Name="ModelHasClass" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="useNullables">
            <DomainPropertyMoniker Name="Model/UseNullables" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="caseOfPrivateFields">
            <DomainPropertyMoniker Name="Model/CaseOfPrivateFields" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="generateMonoRailProject">
            <DomainPropertyMoniker Name="Model/GenerateMonoRailProject" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="monoRailProjectName">
            <DomainPropertyMoniker Name="Model/MonoRailProjectName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="monoRailProjectPath">
            <DomainPropertyMoniker Name="Model/MonoRailProjectPath" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="monoRailDefaultLayout">
            <DomainPropertyMoniker Name="Model/MonoRailDefaultLayout" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="monoRailDefaultRescue">
            <DomainPropertyMoniker Name="Model/MonoRailDefaultRescue" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="monoRailViewFileExtension">
            <DomainPropertyMoniker Name="Model/MonoRailViewFileExtension" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="useGenerics">
            <DomainPropertyMoniker Name="Model/UseGenerics" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="useBaseClass">
            <DomainPropertyMoniker Name="Model/UseBaseClass" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="baseClassName">
            <DomainPropertyMoniker Name="Model/BaseClassName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="generatesDoubleDerived">
            <DomainPropertyMoniker Name="Model/GeneratesDoubleDerived" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="doubleDerivedNameSuffix">
            <DomainPropertyMoniker Name="Model/DoubleDerivedNameSuffix" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="useGeneratedCodeAttribute">
            <DomainPropertyMoniker Name="Model/UseGeneratedCodeAttribute" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="target">
            <DomainPropertyMoniker Name="Model/Target" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="assemblyPath">
            <DomainPropertyMoniker Name="Model/AssemblyPath" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="activeRecordAssemblyName">
            <DomainPropertyMoniker Name="Model/ActiveRecordAssemblyName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="nHibernateAssemblyName">
            <DomainPropertyMoniker Name="Model/NHibernateAssemblyName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="relateWithActiwFile">
            <DomainPropertyMoniker Name="Model/RelateWithActiwFile" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="useVirtualProperties">
            <DomainPropertyMoniker Name="Model/UseVirtualProperties" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="namespace">
            <DomainPropertyMoniker Name="Model/Namespace" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="additionalImports" Representation="Element">
            <DomainPropertyMoniker Name="Model/AdditionalImports" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelClass" MonikerAttributeName="" MonikerElementName="modelClassMoniker" ElementName="modelClass" MonikerTypeName="ModelClassMoniker">
        <DomainClassMoniker Name="ModelClass" />
        <ElementData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="targets">
            <DomainRelationshipMoniker Name="ManyToOneRelation" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="properties">
            <DomainRelationshipMoniker Name="ClassHasProperty" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="cache">
            <DomainPropertyMoniker Name="ModelClass/Cache" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="discriminatorColumn">
            <DomainPropertyMoniker Name="ModelClass/DiscriminatorColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="discriminatorType">
            <DomainPropertyMoniker Name="ModelClass/DiscriminatorType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="discriminatorValue">
            <DomainPropertyMoniker Name="ModelClass/DiscriminatorValue" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="lazy">
            <DomainPropertyMoniker Name="ModelClass/Lazy" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="proxy">
            <DomainPropertyMoniker Name="ModelClass/Proxy" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="schema">
            <DomainPropertyMoniker Name="ModelClass/Schema" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="table">
            <DomainPropertyMoniker Name="ModelClass/Table" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="where">
            <DomainPropertyMoniker Name="ModelClass/Where" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="manyToManyTargets">
            <DomainRelationshipMoniker Name="ManyToManyRelation" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="oneToOneTarget">
            <DomainRelationshipMoniker Name="OneToOneRelation" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="hasKeyProperty" Representation="Ignore">
            <DomainPropertyMoniker Name="ModelClass/HasKeyProperty" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isValidatorSet" Representation="Ignore">
            <DomainPropertyMoniker Name="ModelClass/IsValidatorSet" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="dynamicInsert">
            <DomainPropertyMoniker Name="ModelClass/DynamicInsert" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="dynamicUpdate">
            <DomainPropertyMoniker Name="ModelClass/DynamicUpdate" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="persister">
            <DomainPropertyMoniker Name="ModelClass/Persister" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="selectBeforeUpdate">
            <DomainPropertyMoniker Name="ModelClass/SelectBeforeUpdate" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="polymorphism">
            <DomainPropertyMoniker Name="ModelClass/Polymorphism" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="mutable">
            <DomainPropertyMoniker Name="ModelClass/Mutable" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="batchSize">
            <DomainPropertyMoniker Name="ModelClass/BatchSize" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="locking">
            <DomainPropertyMoniker Name="ModelClass/Locking" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="useAutoImport">
            <DomainPropertyMoniker Name="ModelClass/UseAutoImport" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelHasClass" MonikerAttributeName="" MonikerElementName="modelHasClassMoniker" ElementName="modelHasClass" MonikerTypeName="ModelHasClassMoniker">
        <DomainRelationshipMoniker Name="ModelHasClass" />
      </XmlClassData>
      <XmlClassData TypeName="ManyToOneRelation" MonikerAttributeName="" SerializeId="true" MonikerElementName="manyToOneRelationMoniker" ElementName="manyToOneRelation" MonikerTypeName="ManyToOneRelationMoniker">
        <DomainRelationshipMoniker Name="ManyToOneRelation" />
        <ElementData>
          <XmlPropertyData XmlName="targetCache">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetCache" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetCascade">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetCascade" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetColumnKey">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetColumnKey" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetCustomAccess">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetCustomAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetInverse">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetInverse" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetLazy">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetLazy" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetMapType">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetMapType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetOrderBy">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetOrderBy" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetRelationType">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetRelationType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetSchema">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetSchema" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetSort">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetSort" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetTable">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetTable" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetWhere">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetWhere" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetDescription">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetDescription" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetPropertyName">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetPropertyName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetPropertyType">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetPropertyType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceCascade">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceCascade" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceColumn">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceCustomAccess">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceCustomAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceInsert">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceInsert" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceNotNull">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceNotNull" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceOuterJoin">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceOuterJoin" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceType">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceUnique">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceUnique" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceUpdate">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceUpdate" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceDescription">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceDescription" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetAccess">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourcePropertyName">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourcePropertyName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceNotFoundBehaviour">
            <DomainPropertyMoniker Name="ManyToOneRelation/SourceNotFoundBehaviour" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetNotFoundBehaviour">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetNotFoundBehaviour" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetElement">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetElement" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetIndexType">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetIndexType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetIndex">
            <DomainPropertyMoniker Name="ManyToOneRelation/TargetIndex" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ManyToOneConnector" MonikerAttributeName="" MonikerElementName="manyToOneConnectorMoniker" ElementName="manyToOneConnector" MonikerTypeName="ManyToOneConnectorMoniker">
        <ConnectorMoniker Name="ManyToOneConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ActiveRecordMapping" MonikerAttributeName="" MonikerElementName="activeRecordMappingMoniker" ElementName="activeRecordMapping" MonikerTypeName="ActiveRecordMappingMoniker">
        <DiagramMoniker Name="ActiveRecordMapping" />
      </XmlClassData>
      <XmlClassData TypeName="ClassShape" MonikerAttributeName="" MonikerElementName="classShapeMoniker" ElementName="classShape" MonikerTypeName="ClassShapeMoniker">
        <CompartmentShapeMoniker Name="ClassShape" />
      </XmlClassData>
      <XmlClassData TypeName="ModelProperty" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelPropertyMoniker" ElementName="modelProperty" MonikerTypeName="ModelPropertyMoniker">
        <DomainClassMoniker Name="ModelProperty" />
        <ElementData>
          <XmlPropertyData XmlName="column">
            <DomainPropertyMoniker Name="ModelProperty/Column" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="columnType">
            <DomainPropertyMoniker Name="ModelProperty/ColumnType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="formula">
            <DomainPropertyMoniker Name="ModelProperty/Formula" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="insert">
            <DomainPropertyMoniker Name="ModelProperty/Insert" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="length">
            <DomainPropertyMoniker Name="ModelProperty/Length" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="notNull">
            <DomainPropertyMoniker Name="ModelProperty/NotNull" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="unique">
            <DomainPropertyMoniker Name="ModelProperty/Unique" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="unsavedValue">
            <DomainPropertyMoniker Name="ModelProperty/UnsavedValue" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="update">
            <DomainPropertyMoniker Name="ModelProperty/Update" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="generator">
            <DomainPropertyMoniker Name="ModelProperty/Generator" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="keyType">
            <DomainPropertyMoniker Name="ModelProperty/KeyType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="params">
            <DomainPropertyMoniker Name="ModelProperty/Params" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sequenceName">
            <DomainPropertyMoniker Name="ModelProperty/SequenceName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="accessor">
            <DomainPropertyMoniker Name="ModelProperty/Accessor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="compositeKeyName">
            <DomainPropertyMoniker Name="ModelProperty/CompositeKeyName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="propertyType">
            <DomainPropertyMoniker Name="ModelProperty/PropertyType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="debuggerDisplay">
            <DomainPropertyMoniker Name="ModelProperty/DebuggerDisplay" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="validator">
            <DomainPropertyMoniker Name="ModelProperty/Validator" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="uniqueKey">
            <DomainPropertyMoniker Name="ModelProperty/UniqueKey" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="index">
            <DomainPropertyMoniker Name="ModelProperty/Index" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sqlType">
            <DomainPropertyMoniker Name="ModelProperty/SqlType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="check">
            <DomainPropertyMoniker Name="ModelProperty/Check" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ClassHasProperty" MonikerAttributeName="" MonikerElementName="classHasPropertyMoniker" ElementName="classHasProperty" MonikerTypeName="ClassHasPropertyMoniker">
        <DomainRelationshipMoniker Name="ClassHasProperty" />
      </XmlClassData>
      <XmlClassData TypeName="ModelElementWithAccess" MonikerAttributeName="" MonikerElementName="modelElementWithAccessMoniker" ElementName="modelElementWithAccess" MonikerTypeName="ModelElementWithAccessMoniker">
        <DomainClassMoniker Name="ModelElementWithAccess" />
        <ElementData>
          <XmlPropertyData XmlName="customAccess">
            <DomainPropertyMoniker Name="ModelElementWithAccess/CustomAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="access">
            <DomainPropertyMoniker Name="ModelElementWithAccess/Access" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="NamedElement" MonikerAttributeName="name" MonikerElementName="namedElementMoniker" ElementName="namedElement" MonikerTypeName="NamedElementMoniker">
        <DomainClassMoniker Name="NamedElement" />
        <ElementData>
          <XmlPropertyData XmlName="name" IsMonikerKey="true">
            <DomainPropertyMoniker Name="NamedElement/Name" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="description">
            <DomainPropertyMoniker Name="NamedElement/Description" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ManyToManyRelation" MonikerAttributeName="" SerializeId="true" MonikerElementName="manyToManyRelationMoniker" ElementName="manyToManyRelation" MonikerTypeName="ManyToManyRelationMoniker">
        <DomainRelationshipMoniker Name="ManyToManyRelation" />
        <ElementData>
          <XmlPropertyData XmlName="sourceCache">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceCache" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceCascade">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceCascade" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceColumn">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceCustomAccess">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceCustomAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceInverse">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceInverse" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceLazy">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceLazy" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceMapType">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceMapType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceOrderBy">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceOrderBy" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceRelationType">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceRelationType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="schema">
            <DomainPropertyMoniker Name="ManyToManyRelation/Schema" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceSort">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceSort" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="table">
            <DomainPropertyMoniker Name="ManyToManyRelation/Table" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceWhere">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceWhere" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetCache">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetCache" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetCascade">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetCascade" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetColumn">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetCustomAccess">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetCustomAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetInverse">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetInverse" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetLazy">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetLazy" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetMapType">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetMapType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetOrderBy">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetOrderBy" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetRelationType">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetRelationType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetSort">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetSort" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetWhere">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetWhere" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceDescription">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceDescription" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetDescription">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetDescription" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceAccess">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetAccess">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourcePropertyType">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourcePropertyType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetPropertyType">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetPropertyType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourcePropertyName">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourcePropertyName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetPropertyName">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetPropertyName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetNotFoundBehaviour">
            <DomainPropertyMoniker Name="ManyToManyRelation/TargetNotFoundBehaviour" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceNotFoundBehaviour">
            <DomainPropertyMoniker Name="ManyToManyRelation/SourceNotFoundBehaviour" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ManyToManyConnector" MonikerAttributeName="" MonikerElementName="manyToManyConnectorMoniker" ElementName="manyToManyConnector" MonikerTypeName="ManyToManyConnectorMoniker">
        <ConnectorMoniker Name="ManyToManyConnector" />
      </XmlClassData>
      <XmlClassData TypeName="OneToOneRelation" MonikerAttributeName="" MonikerElementName="oneToOneRelationMoniker" ElementName="oneToOneRelation" MonikerTypeName="OneToOneRelationMoniker">
        <DomainRelationshipMoniker Name="OneToOneRelation" />
        <ElementData>
          <XmlPropertyData XmlName="sourceAccess">
            <DomainPropertyMoniker Name="OneToOneRelation/SourceAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceCascade">
            <DomainPropertyMoniker Name="OneToOneRelation/SourceCascade" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceConstrained">
            <DomainPropertyMoniker Name="OneToOneRelation/SourceConstrained" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceCustomAccess">
            <DomainPropertyMoniker Name="OneToOneRelation/SourceCustomAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceOuterJoin">
            <DomainPropertyMoniker Name="OneToOneRelation/SourceOuterJoin" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetAccess">
            <DomainPropertyMoniker Name="OneToOneRelation/TargetAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetCascade">
            <DomainPropertyMoniker Name="OneToOneRelation/TargetCascade" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetConstrained">
            <DomainPropertyMoniker Name="OneToOneRelation/TargetConstrained" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetCustomAccess">
            <DomainPropertyMoniker Name="OneToOneRelation/TargetCustomAccess" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetOuterJoin">
            <DomainPropertyMoniker Name="OneToOneRelation/TargetOuterJoin" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceDescription">
            <DomainPropertyMoniker Name="OneToOneRelation/SourceDescription" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetDescription">
            <DomainPropertyMoniker Name="OneToOneRelation/TargetDescription" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="OneToOneConnector" MonikerAttributeName="" MonikerElementName="oneToOneConnectorMoniker" ElementName="oneToOneConnector" MonikerTypeName="OneToOneConnectorMoniker">
        <ConnectorMoniker Name="OneToOneConnector" />
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="ActiveWriterExplorer" />
  <ConnectionBuilders>
    <ConnectionBuilder Name="ConnectRelation">
      <Notes>Creates a one to many relationship by pointing at two classes</Notes>
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ManyToOneRelation" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="ClassHasPropertyBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ClassHasProperty" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelProperty" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="ConnectManyRelation">
      <Notes>Creates a many to many relationship by pointing at two classes</Notes>
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ManyToManyRelation" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="OneToOneRelationBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="OneToOneRelation" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
  </ConnectionBuilders>
  <Diagram Id="d25f93b4-c78e-4d22-9d48-ce498e8c1e58" Description="Description for Altinoren.ActiveWriter.ActiveRecordMapping" Name="ActiveRecordMapping" DisplayName="Active Record Mapping" Namespace="Altinoren.ActiveWriter">
    <Class>
      <DomainClassMoniker Name="Model" />
    </Class>
    <ShapeMaps>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="ModelClass" />
        <ParentElementPath>
          <DomainPath>ModelHasClass.Model/!Model</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ClassShape/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ClassShape/Key" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="ModelClass/HasKeyProperty" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="True" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ClassShape/Validation" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="ModelClass/IsValidatorSet" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="True" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="ClassShape" />
        <CompartmentMap>
          <CompartmentMoniker Name="ClassShape/Properties" />
          <ElementsDisplayed>
            <DomainPath>ClassHasProperty.Properties/!Property</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
    </ShapeMaps>
    <ConnectorMaps>
      <ConnectorMap>
        <ConnectorMoniker Name="ManyToOneConnector" />
        <DomainRelationshipMoniker Name="ManyToOneRelation" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ManyToManyConnector" />
        <DomainRelationshipMoniker Name="ManyToManyRelation" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="OneToOneConnector" />
        <DomainRelationshipMoniker Name="OneToOneRelation" />
      </ConnectorMap>
    </ConnectorMaps>
  </Diagram>
  <Designer FileExtension="actiw" EditorGuid="afa967d1-99df-4330-a1d6-a96343c24786">
    <RootClass>
      <DomainClassMoniker Name="Model" />
    </RootClass>
    <XmlSerializationDefinition CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="ActiveWriterSerializationBehavior" />
    </XmlSerializationDefinition>
    <ToolboxTab TabText="ActiveWriter">
      <ElementTool Name="Class" ToolboxIcon="Resources\ClassTool.bmp" Caption="Class" Tooltip="Create a Domain class" HelpKeyword="CreateClassF1Keyword">
        <DomainClassMoniker Name="ModelClass" />
      </ElementTool>
      <ConnectionTool Name="ManyToOneRelationship" ToolboxIcon="Resources\Connector12n.bmp" Caption="Many To One Relationship" Tooltip="Drag between classes to create a many to one relationship" HelpKeyword="ConnectRelationF1Keyword">
        <ConnectionBuilderMoniker Name="ActiveWriter/ConnectRelation" />
      </ConnectionTool>
      <ConnectionTool Name="ManyToManyRelationship" ToolboxIcon="Resources\Connectorm2n.bmp" Caption="Many To Many Relationship" Tooltip="Drag between classes to create a many to many relationship" HelpKeyword="ManyToManyRelationship">
        <ConnectionBuilderMoniker Name="ActiveWriter/ConnectManyRelation" />
      </ConnectionTool>
      <ConnectionTool Name="OneToOneRelationship" ToolboxIcon="Resources\Connector121.bmp" Caption="One To One Relationship" Tooltip="Drag between classes to create a one to one relationship" HelpKeyword="OneToOneRelationship">
        <ConnectionBuilderMoniker Name="ActiveWriter/OneToOneRelationBuilder" />
      </ConnectionTool>
    </ToolboxTab>
    <Validation UsesMenu="true" UsesOpen="true" UsesSave="true" UsesLoad="false" />
    <DiagramMoniker Name="ActiveRecordMapping" />
  </Designer>
  <Explorer ExplorerGuid="762a1a9f-b314-4480-8df1-227a3166ee9c" Title="ActiveWriter">
    <ExplorerBehaviorMoniker Name="ActiveWriter/ActiveWriterExplorer" />
  </Explorer>
</Dsl>