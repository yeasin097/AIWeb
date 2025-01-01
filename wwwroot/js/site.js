// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const canvas = document.getElementById('drawingCanvas');
const ctx = canvas.getContext('2d');
let shapes = [];
let selectedShape = null;
let isDragging = false;
let isDrawing = false;
let freeDrawMode = false;
let lastMousePosition = null;

class Shape {
    constructor(type, x, y, color, options = {}) {
        this.type = type;
        this.x = x;
        this.y = y;
        this.color = color;
        this.options = options;
        this.rotation = 0; // Rotation in degrees
    }

    draw(ctx) {
        ctx.save();
        ctx.translate(this.x, this.y);
        ctx.rotate((this.rotation * Math.PI) / 180);
        ctx.fillStyle = this.color;
        ctx.strokeStyle = this.color;

        if (this.type === 'rectangle') {
            ctx.fillRect(-this.options.width / 2, -this.options.height / 2, this.options.width, this.options.height);
        } else if (this.type === 'circle') {
            ctx.beginPath();
            ctx.arc(0, 0, this.options.radius, 0, Math.PI * 2);
            ctx.fill();
        } else if (this.type === 'square') {
            ctx.fillRect(-this.options.size / 2, -this.options.size / 2, this.options.size, this.options.size);
        } else if (this.type === 'triangle') {
            ctx.beginPath();
            ctx.moveTo(0, -this.options.height / 2);
            ctx.lineTo(-this.options.base / 2, this.options.height / 2);
            ctx.lineTo(this.options.base / 2, this.options.height / 2);
            ctx.closePath();
            ctx.fill();
        } else if (this.type === 'arrow') {
            const { x2, y2 } = this.options;
            const headLength = 15;
            const angle = Math.atan2(y2, x2);
            ctx.beginPath();
            ctx.moveTo(0, 0);
            ctx.lineTo(x2, y2);
            ctx.lineTo(
                x2 - headLength * Math.cos(angle - Math.PI / 6),
                y2 - headLength * Math.sin(angle - Math.PI / 6)
            );
            ctx.moveTo(x2, y2);
            ctx.lineTo(
                x2 - headLength * Math.cos(angle + Math.PI / 6),
                y2 - headLength * Math.sin(angle + Math.PI / 6)
            );
            ctx.stroke();
        }
        ctx.restore();
    }

    contains(x, y) {
        ctx.save();
        ctx.translate(this.x, this.y);
        ctx.rotate((this.rotation * Math.PI) / 180);

        let contains = false;
        if (this.type === 'rectangle' || this.type === 'square') {
            const width = this.type === 'rectangle' ? this.options.width : this.options.size;
            const height = this.type === 'rectangle' ? this.options.height : this.options.size;
            contains = x >= -width / 2 && x <= width / 2 && y >= -height / 2 && y <= height / 2;
        } else if (this.type === 'circle') {
            const dx = x;
            const dy = y;
            contains = Math.sqrt(dx * dx + dy * dy) <= this.options.radius;
        } else if (this.type === 'triangle') {
            const { base, height } = this.options;
            contains = x >= -base / 2 && x <= base / 2 && y >= -height / 2 && y <= height / 2;
        }
        ctx.restore();
        return contains;
    }

    rotate(degrees) {
        this.rotation = (this.rotation + degrees) % 360;
    }

    resize(scaleFactor) {
        if (this.type === 'rectangle') {
            this.options.width *= scaleFactor;
            this.options.height *= scaleFactor;
        } else if (this.type === 'circle') {
            this.options.radius *= scaleFactor;
        } else if (this.type === 'square') {
            this.options.size *= scaleFactor;
        } else if (this.type === 'triangle') {
            this.options.base *= scaleFactor;
            this.options.height *= scaleFactor;
        }
    }
}

function redrawCanvas() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    shapes.forEach(shape => shape.draw(ctx));
}

canvas.addEventListener('mousedown', event => {
    const x = event.offsetX;
    const y = event.offsetY;

    if (freeDrawMode) {
        isDrawing = true;
        lastMousePosition = { x, y };
        ctx.beginPath();
        ctx.moveTo(x, y);
    } else {
        selectedShape = shapes.find(shape => shape.contains(x - shape.x, y - shape.y));
        isDragging = !!selectedShape;
    }
});

canvas.addEventListener('mousemove', event => {
    const x = event.offsetX;
    const y = event.offsetY;

    if (freeDrawMode && isDrawing) {
        ctx.lineTo(x, y);
        ctx.stroke();
        lastMousePosition = { x, y };
    } else if (isDragging && selectedShape) {
        const dx = event.movementX;
        const dy = event.movementY;
        selectedShape.x += dx;
        selectedShape.y += dy;
        redrawCanvas();
    }
});

canvas.addEventListener('mouseup', () => {
    if (freeDrawMode) {
        isDrawing = false;
    } else {
        isDragging = false;
    }
});

document.getElementById('freeDraw').addEventListener('click', () => {
    freeDrawMode = true;
});

document.getElementById('drawRectangle').addEventListener('click', () => {
    freeDrawMode = false;
    const rect = new Shape('rectangle', 150, 150, 'blue', { width: 200, height: 100 });
    shapes.push(rect);
    redrawCanvas();
});

document.getElementById('drawCircle').addEventListener('click', () => {
    freeDrawMode = false;
    const circle = new Shape('circle', 400, 300, 'red', { radius: 50 });
    shapes.push(circle);
    redrawCanvas();
});

document.getElementById('drawSquare').addEventListener('click', () => {
    freeDrawMode = false;
    const square = new Shape('square', 200, 200, 'purple', { size: 100 });
    shapes.push(square);
    redrawCanvas();
});

document.getElementById('drawTriangle').addEventListener('click', () => {
    freeDrawMode = false;
    const triangle = new Shape('triangle', 300, 300, 'orange', { base: 100, height: 100 });
    shapes.push(triangle);
    redrawCanvas();
});

document.getElementById('rotateShape').addEventListener('click', () => {
    if (selectedShape) {
        selectedShape.rotate(15); // Rotate by 15 degrees
        redrawCanvas();
    }
});

document.getElementById('resizeShape').addEventListener('click', () => {
    if (selectedShape) {
        const scaleFactor = prompt('Enter resize factor (e.g., 1.2 for larger, 0.8 for smaller):');
        selectedShape.resize(parseFloat(scaleFactor));
        redrawCanvas();
    }
});

document.getElementById('deleteShape').addEventListener('click', () => {
    if (selectedShape) {
        shapes = shapes.filter(shape => shape !== selectedShape);
        selectedShape = null;
        redrawCanvas();
    }
});

document.getElementById('clearCanvas').addEventListener('click', () => {
    shapes = [];
    redrawCanvas();
});

document.getElementById('downloadImage').addEventListener('click', () => {
    const link = document.createElement('a');
    link.download = 'drawing.png';
    link.href = canvas.toDataURL();
    link.click();
});

document.getElementById('generateAIImage').addEventListener('click', async () => {
    const prompt = document.getElementById('aiPrompt').value;
    const response = await fetch('/api/AiDraw', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ prompt })
    });
    const data = await response.json();
    const img = new Image();
    img.src = 'data:image/png;base64,' + data.imageBase64;
    img.onload = () => ctx.drawImage(img, 0, 0);
});
document.getElementById('generateAIImage').addEventListener('click', async () => {
    const prompt = document.getElementById('aiPrompt').value;
    const response = await fetch('/api/Ai/GenerateImage', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ prompt })
    });

    if (response.ok) {
        const data = await response.json();
        const img = new Image();
        img.src = data.data[0].url; // OpenAI's response contains the image URL
        img.onload = () => ctx.drawImage(img, 0, 0);
    } else {
        alert("Failed to generate AI image.");
    }
});

