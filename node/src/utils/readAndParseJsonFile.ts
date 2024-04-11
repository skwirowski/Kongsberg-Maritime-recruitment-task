import fs from "fs";
import path from "path";

export const readAndParseJsonFile = () => {
    const filePath = path.resolve(__dirname, "../..", "signalReceiverConfig.json");
    let jsonData;

    try {
        const data = fs.readFileSync(filePath, "utf-8");
        jsonData = JSON.parse(data);
    } catch (error) {
        console.error("Error reading JSON file: ", error);
    }

    return jsonData;
};
