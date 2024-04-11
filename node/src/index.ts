import express, { Express, Request, Response } from "express";
import dotenv from "dotenv";
import { receiveSignal } from "./signalReceiver";
import { readAndParseJsonFile } from "./utils/readAndParseJsonFile";

dotenv.config();

const app: Express = express();
const port = process.env.PORT || 3000;

const receiverConfig = readAndParseJsonFile();

// Enable receivers
receiverConfig.Receivers.forEach(
    (config: { id: number; sensorSubscription: string; isActive: boolean }) => {
        if (config.isActive) {
            receiveSignal(config.sensorSubscription);
        }
    },
);

app.get("/", (req: Request, res: Response) => {
    res.send("Express + TypeScript Server");
});

app.listen(port, () => {
    console.log(`[server]: Server is running at http://localhost:${port}`);
});
