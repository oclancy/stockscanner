<Query Kind="Statements" />

var reader = new StreamReader(@"C:\Users\Admin\Downloads\goog-key-statistics.htm");
var html = reader.ReadToEnd();

int startIndex = html.IndexOf("<table width=\"100%\" class=\"yfnc_mod_table_title1\"");

int endIndex = html.IndexOf("</table>");

html = html.Substring(startIndex, html.Length-endIndex);

var xml = XElement.Parse(html);

