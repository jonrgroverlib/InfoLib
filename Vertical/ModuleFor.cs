//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InfoLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InfoLib.  If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
//using System.Web.UI.WebControls;      // for GridView, Label
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    //// --------------------------------------------------------------------------------------------
    ///// <!-- ModuleFor -->
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <remarks>alpha code - example code, to be deprecated</remarks>
    //public static class ModuleFor
    //{

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- AssessmentReason -->
    //    /// <summary>
    //    ///      Parameters for the AssessmentReason module
    //    /// </summary>
    //    /// <param name="relativeRootPathAndNamePrefix">for example "~/Certification/Certification"</param>
    //    /// <returns></returns>
    //    public static CrudModule AssessmentReason(string relativeRootPathAndNamePrefix)
    //    {
    //        if (_reason == null)
    //            _reason = new CrudModule("Assessment Reason");
    //        _reason.Path       = relativeRootPathAndNamePrefix;
    //        _reason.Label      = "Reason";
    //        _reason.OrderBy    = "ReasonName";
    //        _reason.NameColumn = "ReasonName";


    //        if (!_reason.IsUsedBy.ContainsKey("ConsumerAssessment")) try { _reason.IsUsedBy.Add("ConsumerAssessment", "AssessmentReasonId"); } catch { }


    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        // check for existence                                  Create                    Field tag     Create       Grid Location         Display label         Table name               ID column         Field column
    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        GridView grid = new GridView();                                                                                                                                                                  
    //        if (!_reason.ForAdd .ContainsKey("ReasonName"))     try { _reason.ForAdd .Add("ReasonName"    , new WebField(                      "Reason Name"       , "AssessmentReason"     , "ReasonId"     , "ReasonName"          )); } catch { }
    //        if (!_reason.ForEdit.ContainsKey("ReasonName"))     try { _reason.ForEdit.Add("ReasonName"    , new WebField(                      "Reason Name"       , "AssessmentReason"     , "ReasonId"     , "ReasonName"          )); } catch { }
    //        if (!_reason.ForLoad.ContainsKey("ReasonName"))     try { _reason.ForLoad.Add("ReasonName"    , new WebField(new Label(), grid, 1, "Reason Name"       , "AssessmentReason"     , "ReasonId"     , "ReasonName"          )); } catch { }
    //        if (!_reason.ForView.ContainsKey("ReasonName"))     try { _reason.ForView.Add("ReasonName"    , new WebField(                      "Reason Name"       , "AssessmentReason"     , "ReasonId"     , "ReasonName"          )); } catch { }


    //        return _reason;
    //    }
    //    private static CrudModule _reason;


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Certification -->
    //    /// <summary>
    //    ///      Parameters for the certification module
    //    /// </summary>
    //    /// <param name="relativeRootPathAndNamePrefix">this is the folder then '/' then the first part of the name of each module page</param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code - example code, to be deprecated</remarks>
    //    public static CrudModule Certification(string relativeRootPathAndNamePrefix)
    //    {
    //        if (_cert == null)
    //            _cert = new CrudModule("Certification");
    //        _cert.Path       = relativeRootPathAndNamePrefix;
    //        _cert.Label      = "Certification";
    //        _cert.OrderBy    = "Cert_description";
    //        _cert.NameColumn = "Cert_description";


    //        if (!_cert.IsUsedBy.ContainsKey("User_Certification")) try { _cert.IsUsedBy.Add("User_Certification", "cert_ID"); } catch { }


    //        // ---------------------------------------  ----------------------------------  -----------  --------------------  --------------------------------  ----------------------  ---------  ------------------
    //        // check for existence                            Create            Field tag   Create       Grid Location         Display label                     Table name              ID column  Field column
    //        // ---------------------------------------  ----------------------------------  -----------  --------------------  --------------------------------  ----------------------  ---------  ------------------
    //        if (!_cert.ForAdd.ContainsKey ("CertName")) try { _cert.ForAdd.Add ("CertName", new WebField(                      "Certification Name/Description", "Certification_Master", "Cert_ID", "Cert_description")); } catch { }
    //        if (!_cert.ForAdd.ContainsKey ("CertCode")) try { _cert.ForAdd.Add ("CertCode", new WebField(                      "PRAED Certification Code"      , "Certification_Master", "Cert_ID", "Cert_Code"       )); } catch { }
    //        if (!_cert.ForAdd.ContainsKey ("ToolCode")) try { _cert.ForAdd.Add ("ToolCode", new WebField(                      "Tool Code"                     , "Certification_Master", "Cert_ID", "ToolCode"        )); } catch { }
                                                                                                                               
                                                                                                                               
    //        if (!_cert.ForEdit.ContainsKey("CertName")) try { _cert.ForEdit.Add("CertName", new WebField(                      "Certification Name/Description", "Certification_Master", "Cert_ID", "Cert_Description")); } catch { }
    //        if (!_cert.ForEdit.ContainsKey("CertCode")) try { _cert.ForEdit.Add("CertCode", new WebField(                      "PRAED Certification Code"      , "Certification_Master", "Cert_ID", "Cert_Code"       )); } catch { }
    //        if (!_cert.ForEdit.ContainsKey("ToolCode")) try { _cert.ForEdit.Add("ToolCode", new WebField(                      "Tool Code"                     , "Certification_Master", "Cert_ID", "ToolCode"        )); } catch { }


    //        GridView grid = new GridView();
    //        if (!_cert.ForLoad.ContainsKey("CertName")) try { _cert.ForLoad.Add("CertName", new WebField(new Label(), grid, 1, "Certification Name"            , "Certification_Master", "Cert_ID", "Cert_Description")); } catch { }
    //        if (!_cert.ForLoad.ContainsKey("CertCode")) try { _cert.ForLoad.Add("CertCode", new WebField(new Label(), grid, 2, "PRAED Certification Code"      , "Certification_Master", "Cert_ID", "Cert_Code"       )); } catch { }
    //        if (!_cert.ForLoad.ContainsKey("ToolCode")) try { _cert.ForLoad.Add("ToolCode", new WebField(new Label(), grid, 3, "Tool Code"                     , "Certification_Master", "Cert_ID", "ToolCode"        )); } catch { }


    //        if (!_cert.ForView.ContainsKey("CertName")) try { _cert.ForView.Add("CertName", new WebField(                      "Certification Name/Description", "Certification_Master", "Cert_ID", "Cert_Description")); } catch { }
    //        if (!_cert.ForView.ContainsKey("CertCode")) try { _cert.ForView.Add("CertCode", new WebField(                      "PRAED Certification Code"      , "Certification_Master", "Cert_ID", "Cert_Code"       )); } catch { }
    //        if (!_cert.ForView.ContainsKey("ToolCode")) try { _cert.ForView.Add("ToolCode", new WebField(                      "Tool Code"                     , "Certification_Master", "Cert_ID", "ToolCode"        )); } catch { }


    //        return _cert;
    //    }
    //    private static CrudModule _cert;


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ConfigurationSuggestions -->
    //    /// <summary>
    //    ///      Parameters for the ConfigurationDropdown module
    //    /// </summary>
    //    /// <param name="relativeRootPathAndNamePrefix">for example "~/Certification/Certification"</param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code - example code, to be deprecated</remarks>
    //    public static CrudModule ConfigurationSuggestions(string relativeRootPathAndNamePrefix)
    //    {
    //        if (_suggestion == null)
    //            _suggestion = new CrudModule("Configuration Suggestion");
    //        _suggestion.Path       = relativeRootPathAndNamePrefix;
    //        _suggestion.Label      = "Configuration Suggestion";
    //        _suggestion.OrderBy    = "ConfigItemId, DropdownItemOrd";
    //        _suggestion.NameColumn = "DropdownItemLabel";


    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        // check for existence                                  Create                    Field tag     Create       Grid Location         Display label         Table name               ID column         Field column
    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        if (!_suggestion.ForAdd .ContainsKey("ConfigItem")) try { _suggestion.ForAdd .Add("ConfigItem", new WebField(                      "Configuration Item", "ConfigurationDropdown", "DropdownItemId", "ConfigItemId"      )); } catch { }
    //        if (!_suggestion.ForAdd .ContainsKey("ItemOrder" )) try { _suggestion.ForAdd .Add("ItemOrder" , new WebField(                      "Ordinal Number"    , "ConfigurationDropdown", "DropdownItemId", "DropdownItemOrd"   )); } catch { }
    //        if (!_suggestion.ForAdd .ContainsKey("ItemLabel" )) try { _suggestion.ForAdd .Add("ItemLabel" , new WebField(                      "Label"             , "ConfigurationDropdown", "DropdownItemId", "DropdownItemLabel" )); } catch { }
    //        if (!_suggestion.ForAdd .ContainsKey("ItemValue" )) try { _suggestion.ForAdd .Add("ItemValue" , new WebField(                      "String Value"      , "ConfigurationDropdown", "DropdownItemId", "DropdownItemValue" )); } catch { }
    //        if (!_suggestion.ForAdd .ContainsKey("ItemNumber")) try { _suggestion.ForAdd .Add("ItemNumber", new WebField(                      "Numeric value"     , "ConfigurationDropdown", "DropdownItemId", "DropdownItemNumber")); } catch { }


    //        if (!_suggestion.ForEdit.ContainsKey("ConfigItem")) try { _suggestion.ForEdit.Add("ConfigItem", new WebField(                      "Configuration Item", "ConfigurationDropdown", "DropdownItemId", "ConfigItemId"      )); } catch { }
    //        if (!_suggestion.ForEdit.ContainsKey("ItemOrder" )) try { _suggestion.ForEdit.Add("ItemOrder" , new WebField(                      "Ordinal Number"    , "ConfigurationDropdown", "DropdownItemId", "DropdownItemOrd"   )); } catch { }
    //        if (!_suggestion.ForEdit.ContainsKey("ItemLabel" )) try { _suggestion.ForEdit.Add("ItemLabel" , new WebField(                      "Label"             , "ConfigurationDropdown", "DropdownItemId", "DropdownItemLabel" )); } catch { }
    //        if (!_suggestion.ForEdit.ContainsKey("ItemValue" )) try { _suggestion.ForEdit.Add("ItemValue" , new WebField(                      "String Value"      , "ConfigurationDropdown", "DropdownItemId", "DropdownItemValue" )); } catch { }
    //        if (!_suggestion.ForEdit.ContainsKey("ItemNumber")) try { _suggestion.ForEdit.Add("ItemNumber", new WebField(                      "Numeric value"     , "ConfigurationDropdown", "DropdownItemId", "DropdownItemNumber")); } catch { }


    //        GridView grid = new GridView();
    //        if (!_suggestion.ForLoad.ContainsKey("ConfigItem")) try { _suggestion.ForLoad.Add("ConfigItem", new WebField(new Label(), grid, 1, "Configuration Item", "ConfigurationDropdown", "DropdownItemId", "ConfigItemId"      )); } catch { }
    //        if (!_suggestion.ForLoad.ContainsKey("ItemOrder" )) try { _suggestion.ForLoad.Add("ItemOrder" , new WebField(new Label(), grid, 2, "Ordinal Number"    , "ConfigurationDropdown", "DropdownItemId", "DropdownItemOrd"   )); } catch { }
    //        if (!_suggestion.ForLoad.ContainsKey("ItemLabel" )) try { _suggestion.ForLoad.Add("ItemLabel" , new WebField(new Label(), grid, 3, "Label"             , "ConfigurationDropdown", "DropdownItemId", "DropdownItemLabel" )); } catch { }
    //        if (!_suggestion.ForLoad.ContainsKey("ItemValue" )) try { _suggestion.ForLoad.Add("ItemValue" , new WebField(new Label(), grid, 4, "String Value"      , "ConfigurationDropdown", "DropdownItemId", "DropdownItemValue" )); } catch { }
    //        if (!_suggestion.ForLoad.ContainsKey("ItemNumber")) try { _suggestion.ForLoad.Add("ItemNumber", new WebField(new Label(), grid, 5, "Numeric value"     , "ConfigurationDropdown", "DropdownItemId", "DropdownItemNumber")); } catch { }


    //        if (!_suggestion.ForView.ContainsKey("ConfigItem")) try { _suggestion.ForView.Add("ConfigItem", new WebField(                      "Configuration Item", "ConfigurationDropdown", "DropdownItemId", "ConfigItemId"      )); } catch { }
    //        if (!_suggestion.ForView.ContainsKey("ItemOrder" )) try { _suggestion.ForView.Add("ItemOrder" , new WebField(                      "Ordinal Number"    , "ConfigurationDropdown", "DropdownItemId", "DropdownItemOrd"   )); } catch { }
    //        if (!_suggestion.ForView.ContainsKey("ItemLabel" )) try { _suggestion.ForView.Add("ItemLabel" , new WebField(                      "Label"             , "ConfigurationDropdown", "DropdownItemId", "DropdownItemLabel" )); } catch { }
    //        if (!_suggestion.ForView.ContainsKey("ItemValue" )) try { _suggestion.ForView.Add("ItemValue" , new WebField(                      "String Value"      , "ConfigurationDropdown", "DropdownItemId", "DropdownItemValue" )); } catch { }
    //        if (!_suggestion.ForView.ContainsKey("ItemNumber")) try { _suggestion.ForView.Add("ItemNumber", new WebField(                      "Numeric value"     , "ConfigurationDropdown", "DropdownItemId", "DropdownItemNumber")); } catch { }

    //        _suggestion.ForAdd["ConfigItem"].JoinTo("ConfigurationItem", "ConfigItemId", typeof(int), "ConfigItemLabel");


    //        return _suggestion;
    //    }
    //    private static CrudModule _suggestion;


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- County -->
    //    /// <summary>
    //    ///      Parameters for the County CRUD module
    //    /// </summary>
    //    /// <param name="relativeRootPathAndNamePrefix">for example "~/Certification/Certification"</param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code - example code, to be deprecated</remarks>
    //    internal static CrudModule County(string relativeRootPathAndNamePrefix)
    //    {
    //        if (_county == null)
    //            _county = new CrudModule("County");
    //        _county.Path       = relativeRootPathAndNamePrefix;
    //        _county.Label      = "County";
    //        _county.OrderBy    = "Description";
    //        _county.NameColumn = "Description";


    //        if (!_county.IsUsedBy.ContainsKey("Agency_Master"  )) try { _county.IsUsedBy.Add("Agency_Master"  , "County_Id"); } catch { }
    //        if (!_county.IsUsedBy.ContainsKey("Provider_Master")) try { _county.IsUsedBy.Add("Provider_Master", "County_Id"); } catch { }


    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        // check for existence                                  Create                    Field tag     Create       Grid Location         Display label         Table name               ID column         Field column
    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        GridView grid = new GridView();                                                                                                                                                                  
    //        if (!_county.ForAdd .ContainsKey("Description")) try { _county.ForAdd .Add("Description", new WebField(                      "County Name"     , "County"               , "County_ID"     , "Description"      )); } catch { }
    //        if (!_county.ForEdit.ContainsKey("Description")) try { _county.ForEdit.Add("Description", new WebField(                      "County Name"     , "County"               , "County_ID"     , "Description"      )); } catch { }
    //        if (!_county.ForLoad.ContainsKey("Description")) try { _county.ForLoad.Add("Description", new WebField(new Label(), grid, 1, "County Name"     , "County"               , "County_ID"     , "Description"      )); } catch { }
    //        if (!_county.ForView.ContainsKey("Description")) try { _county.ForView.Add("Description", new WebField(                      "County Name"     , "County"               , "County_ID"     , "Description"      )); } catch { }


    //        return _county;
    //    }
    //    private static CrudModule _county;


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Relationship -->
    //    /// <summary>
    //    ///      Parameters for the Relationship CRUD module
    //    /// </summary>
    //    /// <param name="relativeRootPathAndNamePrefix">for example "~/Certification/Certification"</param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code - example code, to be deprecated</remarks>
    //    internal static CrudModule Relationship(string relativeRootPathAndNamePrefix)
    //    {
    //        if (_relationship == null)
    //            _relationship = new CrudModule("Relationship");
    //        _relationship.Path       = relativeRootPathAndNamePrefix;
    //        _relationship.Label      = "Relationship";
    //        _relationship.OrderBy    = "Description";
    //        _relationship.NameColumn = "Description";


    //        if (!_relationship.IsUsedBy.ContainsKey("Caregiver")) try { _relationship.IsUsedBy.Add("Caregiver", "Relationship_Id"); } catch { }


    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        // check for existence                                  Create                    Field tag     Create       Grid Location         Display label         Table name               ID column         Field column
    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        GridView grid = new GridView();
    //        if (!_relationship.ForAdd .ContainsKey("Description")) try { _relationship.ForAdd .Add("Description", new WebField("Relationship", "Relationship", "Relationship_ID", "Description")); }                catch { }
    //        if (!_relationship.ForEdit.ContainsKey("Description")) try { _relationship.ForEdit.Add("Description", new WebField("Relationship", "Relationship", "Relationship_ID", "Description")); }                catch { }
    //        if (!_relationship.ForLoad.ContainsKey("Description")) try { _relationship.ForLoad.Add("Description", new WebField(new Label(), grid, 1, "Relationship", "Relationship", "Relationship_ID", "Description")); }                catch { }
    //        if (!_relationship.ForView.ContainsKey("Description")) try { _relationship.ForView.Add("Description", new WebField("Relationship", "Relationship", "Relationship_ID", "Description")); }                catch { }


    //        return _relationship;
    //    }
    //    private static CrudModule _relationship;


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Referral -->
    //    /// <summary>
    //    ///      Parameters for the Referral CRUD module
    //    /// </summary>
    //    /// <param name="relativeRootPathAndNamePrefix"></param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code - example code, to be deprecated</remarks>
    //    internal static CrudModule Referral(string relativeRootPathAndNamePrefix)
    //    {
    //        if (_referral == null)
    //            _referral = new CrudModule("Referral");
    //        _referral.Path       = relativeRootPathAndNamePrefix;
    //        _referral.Label      = "Referral";
    //        _referral.OrderBy    = "Name";
    //        _referral.NameColumn = "Name";


    //        if (!_referral.IsUsedBy.ContainsKey("Assessment_Header")) try { _referral.IsUsedBy.Add("Assessment_Header", "Referral_Id"); } catch { }


    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        // check for existence                                  Create                    Field tag     Create       Grid Location         Display label         Table name               ID column         Field column
    //        // ---------------------------------------------------  --------------------------------------  -----------  --------------------  --------------------  -----------------------  ----------------  --------------------
    //        GridView grid = new GridView();

    //        if (!_referral.ForAdd .ContainsKey("Name"    )) try { _referral.ForAdd .Add("Name"    , new WebField("Referral Name", "Referral", "Referral_ID", "Name"    )); }                catch { }
    //        if (!_referral.ForAdd .ContainsKey("Address1")) try { _referral.ForAdd .Add("Address1", new WebField("Address1"     , "Referral", "Referral_ID", "Address1")); }                catch { }
    //        if (!_referral.ForAdd .ContainsKey("Address2")) try { _referral.ForAdd .Add("Address2", new WebField("Address2"     , "Referral", "Referral_ID", "Address2")); }                catch { }
    //        if (!_referral.ForAdd .ContainsKey("City"    )) try { _referral.ForAdd .Add("City"    , new WebField("City"         , "Referral", "Referral_ID", "City"    )); }                catch { }
    //        if (!_referral.ForAdd .ContainsKey("State"   )) try { _referral.ForAdd .Add("State"   , new WebField("State"        , "Referral", "Referral_ID", "State"   )); }                catch { }
    //        if (!_referral.ForAdd .ContainsKey("Email"   )) try { _referral.ForAdd .Add("Email"   , new WebField("Email"        , "Referral", "Referral_ID", "Email"   )); }                catch { }
    //        if (!_referral.ForAdd .ContainsKey("Phone"   )) try { _referral.ForAdd .Add("Phone"   , new WebField("Phone"        , "Referral", "Referral_ID", "Phone"   )); }                catch { }

    //        if (!_referral.ForEdit.ContainsKey("Name"    )) try { _referral.ForEdit.Add("Name"    , new WebField("Referral Name", "Referral", "Referral_ID", "Name"    )); }                catch { }
    //        if (!_referral.ForEdit.ContainsKey("Address1")) try { _referral.ForEdit.Add("Address1", new WebField("Address1"     , "Referral", "Referral_ID", "Address1")); }                catch { }
    //        if (!_referral.ForEdit.ContainsKey("Address2")) try { _referral.ForEdit.Add("Address2", new WebField("Address2"     , "Referral", "Referral_ID", "Address2")); }                catch { }
    //        if (!_referral.ForEdit.ContainsKey("City"    )) try { _referral.ForEdit.Add("City"    , new WebField("City"         , "Referral", "Referral_ID", "City"    )); }                catch { }
    //        if (!_referral.ForEdit.ContainsKey("State"   )) try { _referral.ForEdit.Add("State"   , new WebField("State"        , "Referral", "Referral_ID", "State"   )); }                catch { }
    //        if (!_referral.ForEdit.ContainsKey("Email"   )) try { _referral.ForEdit.Add("Email"   , new WebField("Email"        , "Referral", "Referral_ID", "Email"   )); }                catch { }
    //        if (!_referral.ForEdit.ContainsKey("Phone"   )) try { _referral.ForEdit.Add("Phone"   , new WebField("Phone"        , "Referral", "Referral_ID", "Phone"   )); }                catch { }

    //        if (!_referral.ForLoad.ContainsKey("Name"    )) try { _referral.ForLoad.Add("Name"    , new WebField(new Label(), grid, 1, "Referral Name", "Referral", "Referral_ID", "Name"    )); } catch { }
    //        if (!_referral.ForLoad.ContainsKey("Address1")) try { _referral.ForLoad.Add("Address1", new WebField(new Label(), grid, 2, "Address1"     , "Referral", "Referral_ID", "Address1")); } catch { }
    //        if (!_referral.ForLoad.ContainsKey("Address2")) try { _referral.ForLoad.Add("Address2", new WebField(new Label(), grid, 3, "Address2"     , "Referral", "Referral_ID", "Address2")); } catch { }
    //        if (!_referral.ForLoad.ContainsKey("City"    )) try { _referral.ForLoad.Add("City"    , new WebField(new Label(), grid, 4, "City"         , "Referral", "Referral_ID", "City"    )); } catch { }
    //        if (!_referral.ForLoad.ContainsKey("State"   )) try { _referral.ForLoad.Add("State"   , new WebField(new Label(), grid, 5, "State"        , "Referral", "Referral_ID", "State"   )); } catch { }
    //        if (!_referral.ForLoad.ContainsKey("Email"   )) try { _referral.ForLoad.Add("Email"   , new WebField(new Label(), grid, 6, "Email"        , "Referral", "Referral_ID", "Email"   )); } catch { }
    //        if (!_referral.ForLoad.ContainsKey("Phone"   )) try { _referral.ForLoad.Add("Phone"   , new WebField(new Label(), grid, 7, "Phone"        , "Referral", "Referral_ID", "Phone"   )); } catch { }

    //        if (!_referral.ForView.ContainsKey("Name"    )) try { _referral.ForView.Add("Name"    , new WebField("Referral Name", "Referral", "Referral_ID", "Name"    )); }                catch { }
    //        if (!_referral.ForView.ContainsKey("Address1")) try { _referral.ForView.Add("Address1", new WebField("Address1"     , "Referral", "Referral_ID", "Address1")); }                catch { }
    //        if (!_referral.ForView.ContainsKey("Address2")) try { _referral.ForView.Add("Address2", new WebField("Address2"     , "Referral", "Referral_ID", "Address2")); }                catch { }
    //        if (!_referral.ForView.ContainsKey("City"    )) try { _referral.ForView.Add("City"    , new WebField("City"         , "Referral", "Referral_ID", "City"    )); }                catch { }
    //        if (!_referral.ForView.ContainsKey("State"   )) try { _referral.ForView.Add("State"   , new WebField("State"        , "Referral", "Referral_ID", "State"   )); }                catch { }
    //        if (!_referral.ForView.ContainsKey("Email"   )) try { _referral.ForView.Add("Email"   , new WebField("Email"        , "Referral", "Referral_ID", "Email"   )); }                catch { }
    //        if (!_referral.ForView.ContainsKey("Phone"   )) try { _referral.ForView.Add("Phone"   , new WebField("Phone"        , "Referral", "Referral_ID", "Phone"   )); }                catch { }


    //        return _referral;
    //    }
    //    private static CrudModule _referral;

    //}
}