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
using System;                         // for 
using System.Text.RegularExpressions; // for RegexOptions

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RegexTranslate -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>production ready</remarks>
    public static class RegexTranslate
    {


        // ----------------------------------------------------------------------------------------
        /// <!-- GatToDita -->
        /// <summary>
        ///      Converts GUI Action Trace (GAT) strings to
        ///      Darwin Information Typing Architecture (DITA) xml strings
        /// </summary>
        /// <returns></returns>
        /// <remarks>alpha code, good RegexTable use example</remarks>
        public static RegexTable GatToDita { get
        {
            RegexTable c = new RegexTable();


// --------------------------------------- tokenize:
c.Add("@",                                 "MYATSIGN",                         "hide real @'s");
c.Add("   ",                               "@",                                "tokenize with @'s");
c.Add("\r\n",                              "@\r\n",                            "tokenize with @'s");
c.Add(@"^",                                "@\r\n", RegexOptions.Singleline,   "add @ to start");
c.Add(@"$",                                "\r\n@", RegexOptions.Singleline,   "add @ to end");
// --------------------------------------- list rows and preprocessing:
c.Add(@"\[\[\(row (<#>|[0-9]+)\)\]\]",     "<step>Double click a row.</step>", "");
c.Add(@"\[\(row (<#>|[0-9]+)\)\]",         "<step>Select a row.</step>",       "");
c.Add("<#>",                               "some number of",                   "");
// --------------------------------------- lists:
c.Add(@"\|RMB > <([^>@]+)> > ([^>@]+)\|",  "@<step>Right click on the appropriate $1 from the list and select '$2'.</step>",                                "");
c.Add(@"\|RMB > ([^>@]+) > ([^>@]+)\|",    "@<step>Right click on $1 from the list and select '$2'.</step>",                                "");
// --------------------------------------- screens:
c.Add(@"(\-\-@\r\n@\r\n)\|([^\[\]@]+)",    "$1<step>Starting on the $2 page,</step>", "");
c.Add(@"\|([^@]+) '[^'@]+'",               "@<step>On the '$1' page,</step>",  "");
c.Add(@"\|\|([^[@]+)",                     "@<step>In the '$1' panel,</step>", "");
c.Add(@"\|([^@]+)",                        "@Now on the '$1' page.",           "");
// --------------------------------------- trees:
c.Add(@"\[\+\] <([^<>@]+)> > ([^<>@]+) > \[\[([^<>@]+)\]\]", "@<step>Navigate using the tree structure selecting $1, then $2, then double click '$3'.</step>",                                "");
c.Add(@"\[\+\] ([^<>@]+) > <([^<>@]+)> > \[\[([^<>@]+)\]\]", "@<step>Navigate using the tree structure selecting $1, then $2, then double click '$3'.</step>",                                "");
c.Add(@"\[\+\] ([^<>@]+) > ([^<>@]+) > \[\[([^<>@]+)\]\]", "@<step>Navigate using the tree structure selecting $1, then $2, then double click '$3'.</step>",                                "");
// --------------------------------------- menus:
c.Add(@"\[([^>@]+) > ([^>@]+) > ([^>@]+) > ([^>@]+)\]", "@<step>On the $1 menu select '$2', then '$3', then '$4'.</step>",                                "");
c.Add(@"\[([^>@]+) > ([^>@]+) > ([^>@]+)\]", "@<step>On the $1 menu select '$2', then '$3'.</step>",                                "");
c.Add(@"\[([^>@]+) > ([^>@]+)\]",          "@<step>Select $2 from the $1 menu.</step>",                                "");
// --------------------------------------- drops:
c.Add(@"([^:@]+): \[<([^<>@]+)>\\\[\]/\]", "@<step>Select a $2 from the $1 dialog.</step>",                                "");
c.Add(@"\(([^:@]+)\): \[<([^_@]+)>\\/\]",  "@<step>Select a $2 from the $1 dropdown.</step>",                                "");
c.Add(@"([^:@]+): \[<([^_@]+)>\\/\]",      "@<step>Select a $2 from the $1 dropdown.</step>",                                "");
// --------------------------------------- fields:
c.Add(@"([^:@]+): \[_\[\^(.)\]_\]",        "@<step>Type control-$2 in the $1 field.</step>",                                "");
c.Add(@"([^:@]+): \[_<([^_@]+)>_\]",       "@<step>Enter a $2 in the $1 field.</step>",                                "");
c.Add(@"([^:@]+): \[_([^<_>@]+)_\]",       "@<step>Enter $2 in the $1 field.</step>",                                "");
// --------------------------------------- keys:
c.Add(@"\[\^(.)\]",                        "@<step>Type control-$1.</step>",   "");
// --------------------------------------- buttons:
c.Add(@"_\[X\]",                           "@<step>Close the page by clicking the X in the upper right hand corner.</step>",                                "");
c.Add(@"_\[([^\[\]@]+)\]",                 "@<step>Click the $1 button to continue.</step>",                                "");
c.Add(@"\[([^\]@]+)]",                     "@<step>Click the $1 button.</step>", "");


// --------------------------------------- fix <step> tags:
c.Add(@"-- TO ([^<>]+) --",                "<title>To $1</title>",             "insert title tags");
c.Add(@",</step>[ \r\n@]+<step>",          ", ",                               "remove step breaks after commas");
c.Add(@"</step>([^<>]+)\.[ \r\n@]+<step>", "$1.</step><step>",                 "draw trailing step info before periods into step");
c.Add(@"</step>([^<>]+)\.[ \r\n@]+<step>", "$1.</step><step>",                 "draw trailing step info before periods into step");
c.Add(@"@([\r\n ]*)([^<@>]+)\.@*<step>",   "$1@<step>$2, ",                    "remove leading problems");
//list.Add("[[BREAK]]", "developer's breakpoint");
c.Add(@", [A-Z]",                          RegexStep.ConvertToLowerCase,       "set clause openings to lower case");
c.Add(@"<step>On ([^<@>]+) page,</step>",  "<step><join />You should now be on $1 page.</step>", "");
c.Add(@" *</step>[@\r\n]*<step><join />",  " ",                                "chop out marked step boundaries");


// --------------------------------------- decant tokenizers:
c.Add("@+\r\n",                            "\r\n",                             "remove tokenizer @'s");
c.Add("@+",                                " ",                                "remove tokenizer @'s");
c.Add("MYATSIGN",                          "@",                                "decant real @'s");


// --------------------------------------- spacing:
c.Add("^ +",                               "", RegexOptions.Multiline,         "remove leading line spaces");
c.Add(@"<step>([^\r\n<]+)[\r\n]+([^\r\n<]+)</step>", "<step>$1 $2</step>",     "get rid of line breaks within a step");


// --------------------------------------- <cmd>, <steps>, <taskbody>, <task> tags:
c.Add(@"<step>([^<]+)</step>",             "<step><cmd>$1</cmd></step>",       "insert cmd tags");
c.Add(@"@",                                "MYATSIGN",                         "hide real @'s");
c.Add(@"^",                                "@\r\n",                            "add @ to start");
c.Add(@"$",                                "\r\n@", RegexOptions.Singleline,   "add @ to end");
c.Add(@"(.)@(.)",                          "$1$2",  RegexOptions.Singleline,   "remove all internal @'s");
c.Add(@"([\r\n\ ]+)<title>",               "@$1<title>",                       "add @ before title");
c.Add(@"</title>([\r\n\ ]+)",              "</title>$1@",                      "add @ after title");
c.Add(@"</cmd></step>(\r\n| )*([^<\r\n]+)(\r\n| )*@", " $2</cmd></step>$3@",       "absorb stuff from last step to @ into step");
c.Add(@"@([\r\n ]*)<step>([^@]+)</step>([\r\n ]*)@", "$1@<steps><step>$2</step></steps>@$3", "insert steps tag");
c.Add(@"<steps>([^@]+)</steps>",           "<taskbody><steps>$1</steps></taskbody>", "insert taskbody tag");
c.Add(@"(</title>[\r\n ]+)@",              "$1",                               "insert @ after title");
c.Add(@"(<title>[^@]+</taskbody>)",        "<task>$1</task>",                  "insert task tag");
c.Add(@"@",                                "",                                 "remove tokenizer @'s");
c.Add(@"MYATSIGN",                         "@",                                "decant real @'s");


// --------------------------------------- spacing:
c.Add(@"</step> *<step>",                  "</step>\r\n<step>",                "insert line breaks between steps");


            return c;
        } }


        // ----------------------------------------------------------------------------------------
        /// <!-- DitaToHtml -->
        /// <summary>
        ///      Converts a DITA xml formatted string to an HTML string
        /// </summary>
        /// <remarks>alpha code, good RegexTable use example</remarks>
        public static RegexTable DitaToHtml { get
        {
            RegexTable d2h = new RegexTable();


            d2h.Add("<step><cmd>",   "<li>",           "");
            d2h.Add("</cmd></step>", "</li>",          "");
            d2h.Add("<steps>",       "<ol>",           "");
            d2h.Add("</steps>",      "</ol>",          "");
            d2h.Add("<title>",       "<h2>",           "");
            d2h.Add("</title>",      "</h2>",          "");
            d2h.Add("^",             "<html><body>",   "");
            d2h.Add("$",             "</body></html>", "");


            return d2h;
        } }


    }
}
