import sys
import argparse
import xml.etree.ElementTree as ET
SubElement = ET.SubElement
import io


root = ET.Element(
    "package", {"xmlns": r"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"})
tree = ET.ElementTree(root)
meta = SubElement(root, "metadata")
id, version, authors, owners, licenseUrl, projectUrl, iconUrl, requireLicenseAcceptance, description, tags, dependencies = map(lambda x: SubElement(meta, x), [
    "id",
    "version",
    "authors",
    "owners",
    "licenseUrl",
    "projectUrl",
    "iconUrl",
    "requireLicenseAcceptance",
    "description",
    "tags",
    "dependencies"
])

id.text = "EpochLauncher"
version.text = sys.argv[1]
authors.text = owners.text = "Peep"
requireLicenseAcceptance.text = "false"
description.text = "stuff"
tags.text = "tag"

empty = []

for child in meta:
	if(child.text is None):
		empty.append(child)
for e in empty:
	meta.remove(e)

def indent(elem, level=0):
    i = "\n" + level*"  "
    if len(elem):
        if not elem.text or not elem.text.strip():
            elem.text = i + "  "
        if not elem.tail or not elem.tail.strip():
            elem.tail = i
        for elem in elem:
            indent(elem, level+1)
        if not elem.tail or not elem.tail.strip():
            elem.tail = i
    else:
        if level and (not elem.tail or not elem.tail.strip()):
            elem.tail = i

indent(root)

tree.write(open(sys.argv[2], "wb"), "utf-8", xml_declaration=True)
