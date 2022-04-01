import { Box, Center, Grid, Image, Link, Text, VStack } from "@chakra-ui/react";
import * as React from "react";
import { FC, ReactNode, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { ToastContainer } from 'react-toastify';
import logo from '../../images/logo_light.svg';
import { getButtonTextColorFromBg } from '../../util/color';
import useInterval from '../../util/useInterval';
import bitcoin from '../bitcoin.svg';
import { createLnInvoice, fetchInvoice, fetchInvoiceIsPaid, Invoice, LightningInvoice } from './invoiceRepo';
import QRCode from "./QR";
import { formatCurrency } from "./utils";
import Button from '../Button';

const userColor = "#fdaa26";

const InvoiceComponent: FC = () => {
    let { id } = useParams<{ id: string }>();
    const [error, setError] = useState<boolean>();
    const [isExpired, setExpired] = useState<boolean>();
    const [copied, setCopied] = useState<boolean>();
    let [invoice, setInvoice] = useState<Invoice>();
    const [lnInvoice, setLnInvoice] = useState<LightningInvoice>();
    const [isPaid, setIsPaid] = useState<boolean>();
    useEffect(() => {
        getInvoice();
    }, [])
    useEffect(() => {
        if (!invoice) {
            return;
        }
        refreshLnInvoice();
    }, [invoice]);
    useInterval(() => checkInvoiceStatus(), 2000);

    const checkInvoiceStatus = async () => {
        if (isPaid)
            return;
        if (invoice && !isExpired) {
            try {
                const result = await fetchInvoiceIsPaid(invoice.strikeInvoiceId);
                if (result.paid) {
                    setIsPaid(true);
                    return;
                }
            }
            catch (e) {
                console.error(e);
            }
        }
    }
    const getInvoice = async () => {
        const result = await fetchInvoice(id);
        if (result.paid) {
            setIsPaid(true);
        }
        setInvoice(result);
    }
    const refreshLnInvoice = async () => {
        if (isPaid) {
            return;
        }
        const result = await createLnInvoice(invoice!.strikeInvoiceId);
        setExpired(false);
        result.expirationInSeconds -= 10;
        setLnInvoice(result);
        setTimeout(() => setExpired(true), (result.expirationInSeconds) * 1000)
    }

    const ReacherName = () => <Text as="span" fontWeight="800" color={userColor}>{invoice?.reachable.name}</Text>

    let inner: ReactNode = null;
    if (error) {
        inner = <Text fontSize="lg" mb={4}>
            Error occured. Please try again later.
        </Text>
    }
    else if (isPaid) {
        inner = <VStack spacing={4}>
            <Text fontSize="lg" style={{ textAlign: "center" }}>
                Thank you. Your email has been delivered to <ReacherName />.
            </Text>
            <Text>
                <Link
                    href="/"
                    isExternal
                    color={userColor}
                >
                    Click here
                </Link>{" "}
                to get your own <b>Reacher</b> email address.
            </Text>
        </VStack>
    }
    else if (!invoice || !lnInvoice) {
        inner = <Text fontSize="lg" mb={4} style={{ textAlign: "center" }}>
            Loading details. Please wait...
        </Text>
    }
    else {
        const displayAmount = formatCurrency({
            amount: invoice.costUsd,
            currency: "USD",
            locales: window.navigator.language,
        });

        inner =
            <Box>
                <VStack spacing={4}>
                    <Box>
                        <Link href="/"><Image src={logo} height="100px" display="inline" /></Link>
                    </Box>

                    <Text fontSize="xl" style={{ textAlign: "center" }}>
                        Tip <ReacherName /> <b>{displayAmount}</b>
                    </Text>
                    <Text style={{ textAlign: "center" }}>
                        then Reacher will deliver your email
                    </Text>
                <Text fontSize='sm' pt={2} color="rgba(255,255,255,0.6)">Just <b>scan</b> or <b>click</b> the QR code below</Text>
                    <Box mt={[3, 0]}>
                        <a href={`lightning:${lnInvoice.lnInvoiceId}`}>
                            {lnInvoice.expirationInSeconds && (
                                <QRCode key={lnInvoice.lnInvoiceId}
                                    color={userColor}
                                    expired={isExpired}
                                    data={lnInvoice.lnInvoiceId}
                                    animationDuration={lnInvoice.expirationInSeconds}
                                />
                            )}
                        </a>
                    </Box>
                    <Text fontSize='sm' color="rgba(255,255,255,0.6)"> or paste it in your <Image src={bitcoin} height="1.3rem" display="inline" /> Bitcoin lightning wallet</Text>
                    <Box>
                        {isExpired ? (
                            <Button onClick={refreshLnInvoice}>
                                Refresh
                            </Button>
                        ) : (
                            <Button
                                onClick={() => {
                                    navigator.clipboard.writeText(lnInvoice.lnInvoiceId);
                                    setCopied(true);
                                    setTimeout(() => setCopied(false), 2000);
                                }}
                            >
                                {copied ? "Copied" : "Copy To Clipboard"}
                            </Button>
                        )}
                    </Box>
                    <Text fontSize="sm" pt={4}>
                        All tips go <b>directly to <ReacherName /></b>
                    </Text>
                    <Text fontSize="sm" textAlign="center">
                        New to <Image src={bitcoin} height="1.3rem" display="inline" /> Bitcoin?{" "}
                        <Link
                            href="https://invite.strike.me/5AL8KE"
                            isExternal
                            color={userColor}
                        >
                            Click here
                        </Link>{" "}
                        to get started with <b>Strike</b>.
                    </Text>
                    <Center pb={6} pt={4} fontSize="sm">
                        <Link mr={2}
                            href="https://strike.me/en/legal/privacy"
                            isExternal
                        >
                            Privacy Notice
                        </Link>{"  |  "}
                        <Link ml={2}
                            href="https://strike.me/en/legal/tos"
                            isExternal
                        >
                            Terms of Service
                        </Link>
                    </Center>
                </VStack>
            </Box>
    }

    return (
        <>
            <Grid minH="100vh" p={3}>
                <Center>
                    {inner}
                </Center>
            </Grid>
            <ToastContainer />
        </>
    );
}
export default InvoiceComponent;