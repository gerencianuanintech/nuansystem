from __future__ import annotations

import zipfile
import xml.etree.ElementTree as ET

path = r"E:\OneDrive\Empresas\Umco\Campos nuevos.xlsx"

ns = {
    "a": "http://schemas.openxmlformats.org/drawingml/2006/main",
    "xdr": "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing",
    "main": "http://schemas.openxmlformats.org/spreadsheetml/2006/main",
}

with zipfile.ZipFile(path) as zf:
    for name in [
        "xl/worksheets/sheet1.xml",
        "xl/worksheets/sheet2.xml",
        "xl/worksheets/sheet3.xml",
    ]:
        root = ET.fromstring(zf.read(name))
        print(f"--- {name}")
        for cell in root.findall(".//main:c", ns):
            ref = cell.attrib.get("r", "")
            text = ""
            inline = cell.find("main:is", ns)
            value = cell.find("main:v", ns)
            if inline is not None:
                text = "".join(t.text or "" for t in inline.findall(".//main:t", ns))
            elif value is not None:
                text = value.text or ""
            if text:
                print(f"CELL {ref}: {text}")

    for name in [
        "xl/drawings/drawing1.xml",
        "xl/drawings/drawing2.xml",
        "xl/drawings/drawing3.xml",
    ]:
        root = ET.fromstring(zf.read(name))
        print(f"--- {name}")
        for anchor in list(root):
            from_el = anchor.find("xdr:from", ns)
            coord = ""
            if from_el is not None:
                row = int(from_el.find("xdr:row", ns).text or "0") + 1
                col = int(from_el.find("xdr:col", ns).text or "0") + 1
                coord = f"r{row}c{col}"

            paragraphs = []
            for paragraph in anchor.findall(".//a:p", ns):
                text = "".join(t.text or "" for t in paragraph.findall(".//a:t", ns))
                if text:
                    paragraphs.append(text)

            if paragraphs:
                print(f"{coord}: {' | '.join(paragraphs)}")
