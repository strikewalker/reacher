import * as React from "react";
//import qrCodeConfig from "./qrCodeConfig";
import styles from "./QRCode.module.css";
import QR from 'qrcode.react';
import lightning from '../lightning.svg';
import { Center } from '@chakra-ui/react'

const QRCode: React.FC<{ data: string, color: string, expired?: boolean, animationDuration: number, onClick?: (a: any) => any }> = ({
    data,
    color,
    expired,
    animationDuration,
    onClick,
}) => {
    const ref = React.useRef<any>();

    //useEffect(() => {
    //  qrCodeConfig.append(ref.current);
    //}, []);

    //useEffect(() => {
    //  qrCodeConfig.update({ data });
    //}, [data]);
    const imageSettings = {
        src: lightning,
        height: 48,
        width: 48,
        excavate: true,
    };
    return (
        <div className={styles.root}>
            <div className={styles.svgBorderContainer} onClick={onClick}>
                <svg width="240px" height="240px" viewBox="0 0 240 240">
                    <rect
                        x="2"
                        y="2"
                        width="236"
                        height="236"
                        fill="none"
                        stroke={color}
                        strokeWidth="4"
                        rx="28"
                    />
                </svg>
                <svg width="240px" height="240px" viewBox="0 0 240 240">
                    <rect
                        x="2"
                        y="2"
                        width="236"
                        height="236"
                        fill="none"
                        stroke="#1A1A1A"
                        strokeWidth="6"
                        strokeDashoffset={960}
                        strokeDasharray={960}
                        rx="28"
                        style={{ animationDuration: (expired ? 0 : `${animationDuration}s`) as any }}
                    />
                </svg>
            </div>
            <div
                className={`${styles.qrCodeContainer} ${expired ? styles.expired : undefined
                    }`}
                ref={ref}
            >
                <Center bg="white" borderRadius={8} boxSizing="border-box" p={2}>
                    <QR
                        value={`lightning:${data}`}
                        size={200}
                        bgColor={"#ffffff"}
                        fgColor={"#000000"}
                        level={"L"}
                        includeMargin={false}
                        renderAs={"svg"}
                        imageSettings={imageSettings}
                    />
                </Center>
            </div>
        </div>
    );
}
export default QRCode;