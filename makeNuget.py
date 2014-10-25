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
licenseUrl.text = ""
projectUrl.text = ""
requireLicenseAcceptance.text = "false"
description.text = "stuff"
tags.text = "tag"
tree.write(open("EpochLauncher.nuspec", "wb"), "utf-8", xml_declaration=True)
