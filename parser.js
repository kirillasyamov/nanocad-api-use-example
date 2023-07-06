const fs = require('fs'), readline = require('readline');
const mode = [process.argv[2] === '--data', process.argv[2] === '--sql'];
const points = [], polylines = [];
let current, commands = {};

class Point {
    constructor(row) {
        this.id = points.length + 1;
        this.x = Number(row.match(/X = (-?\d+\.?\d*)/)[1]);
        this.y = Number(row.match(/Y = (-?\d+\.?\d*)/)[1]);
        this.z = Number(row.match(/Z = (-?\d+\.?\d*)/)[1]);
    }
}

class Polyline {
    static update(row) {
        if (!current) current = new Polyline;
        if (!points.length) return;
        if (/new line/.test(row) && !current.end && !!current.start) {
            current.end = points.at(-1).id;
            polylines.push(current);
            current = new Polyline();
        } else if (!current.start) current.start = points.at(-1).id;

    }
}

function parseCoordinates(link) {
    const stream = fs.createReadStream(link);
    const rl = readline.createInterface({ input: stream });

    rl.on('line', (row) => {
        if (!/new line/.test(row)) points.push(new Point(row));
        Polyline.update(row);
    });

    rl.on('close', () => {
        const coordinates = points.map((point) => `(${point.x},${point.y},${point.z})`).join(', ');
        const lines = polylines.map(line => `(${line.start},${line.end})`).join(', ');

        commands.points = `insert into point (x, y, z) values ${coordinates};`;
        commands.lines = `insert into polyline (first, last) values ${lines};`;

        fs.writeFile('output.json', JSON.stringify(commands), (err) => {
            console.log('Writing ...');
            if (err) throw err;
            console.log('Object saved to file');
        });
    })
}

function parseCommands(link) {
    const commands = require('./dml.json');
    const stream = fs.createReadStream(link);
    const rl = readline.createInterface({ input: stream });
    let output = '';

    rl.on('line', (row) => {
        output += row;
    });

    rl.on('close', () => {
        output += commands.points;
        output += commands.lines;

        fs.writeFile('pepe.sql', output, (err) => {
            console.log('Writing ...');
            if (err) throw err;
            console.log('Object saved to file');
        });
    })
}

if (mode[0]) parseCoordinates('./data.md');

if (mode[1]) parseCommands('./ddl.sql');
