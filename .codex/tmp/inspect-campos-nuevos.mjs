import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const inputPath = "E:/OneDrive/Empresas/Umco/Campos nuevos.xlsx";
const input = await FileBlob.load(inputPath);
const workbook = await SpreadsheetFile.importXlsx(input);

const overview = await workbook.inspect({
  kind: "workbook,sheet,table,region",
  maxChars: 20000,
  tableMaxRows: 40,
  tableMaxCols: 20,
  tableMaxCellChars: 180,
});

console.log(overview.ndjson);
