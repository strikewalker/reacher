import QRCodeStyling from "qr-code-styling";
import lightning from '../lightning.svg';

const qrCodeConfig = new QRCodeStyling({
    width: 200,
    height: 200,
    qrOptions: { typeNumber: "0", mode: "Byte", errorCorrectionLevel: "L" },
    imageOptions: { hideBackgroundDots: true, imageSize: 1.3, margin: 5 },
    dotsOptions: { type: "rounded", color: "#ffffff" },
    backgroundOptions: { color: "#1A1A1A" },
    image: "/images/lightning_logo.png",
    cornersSquareOptions: { type: "extra-rounded", color: "#ffffff" },
    cornersSquareOptionsHelper: {
        colorType: { single: true, gradient: false },
        gradient: {
            linear: true,
            radial: false,
            color1: "#000000",
            color2: "#000000",
            rotation: "0",
        },
    },
    cornersDotOptions: { type: "", color: "#ffffff" },
    cornersDotOptionsHelper: {
        colorType: { single: true, gradient: false },
        gradient: {
            linear: true,
            radial: false,
            color1: "#000000",
            color2: "#000000",
            rotation: "0",
        },
    },
    backgroundOptionsHelper: {
        colorType: { single: true, gradient: false },
        gradient: {
            linear: true,
            radial: false,
            color1: "#ffffff",
            color2: "#ffffff",
            rotation: "0",
        },
    },
});

export default qrCodeConfig;
