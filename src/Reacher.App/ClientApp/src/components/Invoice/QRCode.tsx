import { useEffect, useRef } from 'react';
import * as React from "react";
//import qrCodeConfig from "./qrCodeConfig";
import QR from 'qrcode.react';
import styles from "./QRCode.module.css";
import lightning from '../lightning.svg';

export default function QRCode({ data, animationDuration }: { data: string, animationDuration: number }) {
    const ref = useRef<HTMLDivElement>();

    //useEffect(() => {
    //    qrCodeConfig.append(ref.current);
    //}, []);

    //useEffect(() => {
    //    qrCodeConfig.update({ data });
    //}, [data]);
    const imageSettings = {
        src: lightning,
        height: 48,
        width: 48,
        excavate: true,
    };
    return (
        <div className={styles.root}>
            <div className={styles.svgBorderContainer}>
                <a href={`lightning:${data}`}>
                    <svg width="240px" height="240px" viewBox="0 0 240 240">
                        <rect
                            x="2"
                            y="2"
                            width="236"
                            height="236"
                            fill="none"
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
                            style={{ animationDuration: `${animationDuration}s` }}
                        />
                    </svg>
                </a>
            </div>
            <div className={styles.qrCodeContainer}>
                <QR
                    value={`lightning:${data}`}
                    size={240}
                    bgColor={"#ffffff"}
                    fgColor={"#000000"}
                    level={"L"}
                    includeMargin={false}
                    renderAs={"svg"}
                />
            </div>
        </div>
    );
}
