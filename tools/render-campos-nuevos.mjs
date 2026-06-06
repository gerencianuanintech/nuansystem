import fs from "node:fs/promises";
import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const inputPath = "E:/OneDrive/Empresas/Umco/Campos nuevos.xlsx";
const outputDir = "outputs/campos-nuevos-render";

await fs.mkdir(outputDir, { recursive: true });

const input = await FileBlob.load(inputPath);
const workbook = await SpreadsheetFile.importXlsx(input);
const sheets = await workbook.inspect({ kind: "sheet", include: "id,name", maxChars: 4000 });
console.log(sheets.ndjson);

for (const sheet of ["Hoja1", "Hoja2", "Hoja3"]) {
  const preview = await workbook.render({
    sheetName: sheet,
    autoCrop: "all",
    scale: 2,
    format: "png",
  });
  await fs.writeFile(
    `${outputDir}/${sheet}.png`,
    new Uint8Array(await preview.arrayBuffer()),
  );
}
