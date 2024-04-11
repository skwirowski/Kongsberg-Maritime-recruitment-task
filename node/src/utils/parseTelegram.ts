import chalk from "chalk";

const parseString = (str: string) => {
    const removeFirstAndLastChar = str.substring(1, str.length - 1);
    const splitStr = removeFirstAndLastChar.split(", ");

    return splitStr;
};

const createMessage = (splitStr: string[]) => {
    const [encoderType, id, type, value, quality] = splitStr;
    let setColor;

    switch (quality) {
        case "Alarm":
            setColor = chalk.bgRed.bold;
            break;
        case "Warning":
            setColor = chalk.bgYellow.bold;
            break;
        default:
            setColor = chalk.bgGreen.bold;
    }

    const message = `Encoder type: ${chalk.blue.bold(encoderType)}, Sensor id: ${chalk.blue.bold(id)}, Sensor type: ${chalk.blue.bold(type)}, Reading value: ${chalk.blue.bold(value)}, Signal quaity: ${setColor(quality)}`;

    return message;
};

export const parseTelegram = (telegram: string) => {
    const splitTelegram = parseString(telegram);
    const processedMessage = createMessage(splitTelegram);

    return processedMessage;
};
