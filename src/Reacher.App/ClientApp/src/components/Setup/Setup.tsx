import {
    FormControl, FormHelperText, FormLabel, Input, InputGroup,
    InputLeftAddon,
    InputRightAddon, Link, SimpleGrid, Switch, Text, VStack,
    Heading, Center
} from "@chakra-ui/react";
import * as React from 'react';
import Button from '../Button';
import { orangeColor, reacherSuffix } from "../Common";
import { getCurrency, getReacherEmailAvailable, SetupConfig, updateSetupConfig } from '../Services/userRepo';

const Setup: React.FC<{ setupConfig: SetupConfig, onComplete: (saved?: boolean) => any, onEdit: () => any, edit: boolean }> = ({ setupConfig: sc, onComplete, onEdit, edit }) => {
    let [reacherPrefixInvalid, setReacherPrefixInvalid] = React.useState(false);
    let [reacherPrefixTaken, setReacherPrefixTaken] = React.useState(false);
    let [usernameInvalid, setUsernameInvalid] = React.useState(false);
    let [isSaving, setSaving] = React.useState(false);
    let [error, setError] = React.useState(false);
    let [setupConfig, setSetupConfigTemp] = React.useState<SetupConfig>(sc);
    const setSetupConfig = (action: (config: SetupConfig) => SetupConfig) => {
        setSetupConfigTemp(action as any)
    }
    const handleSubmit = async (event: React.FormEvent<any>) => {
        event.preventDefault();
        try {
            setSaving(true);
            setError(false);
            await updateSetupConfig(setupConfig);
            onComplete(true);
        }
        catch (e) {
            console.error(e);
            setError(true);
        }
        finally {
            setSaving(false);
        }
    }
    const updateCurrency = async () => {
        const currency = await getCurrency(setupConfig!.strikeUsername!);
        setUsernameInvalid(!currency);
        if (currency) {
            setSetupConfig(c => ({ ...c, currency }))
        }
    }
    const checkReacherEmail = async () => {
        setReacherPrefixTaken(false);
        setReacherPrefixInvalid(false);
        if (!setupConfig.reacherEmailPrefix) {
            return;
        }
        const patternMatches = /[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*/.test(setupConfig!.reacherEmailPrefix!);
        if (!patternMatches) {
            setReacherPrefixInvalid(true);
            return;
        }
        const available = await getReacherEmailAvailable(setupConfig!.reacherEmailPrefix!);
        setReacherPrefixTaken(!available);
    }
    const isNew = !sc.reacherEmailPrefix;
    const reacherPrefix = setupConfig!.reacherEmailPrefix ?? "";
    const tipAmount = setupConfig!.price?.toFixed(2);
    return (<>
        <VStack spacing={8}>
            <Center><Heading as="h2" size="md">
                {edit ? "Update " : isNew ? "Set Up " : ""} Your Reacher Profile
            </Heading></Center>
            {edit ? (<Text mb={8} fontSize="sm">
                To set up your Reacher email address, provide the details below, and hit 'Submit' to save your changes.
            </Text>) : (<Text mb={8} fontSize="sm">
                See your Reacher profile below. Hit 'Update Profile' to change any of the values below.
            </Text>)}
            <form onSubmit={handleSubmit}>
                <VStack spacing={4}>
                    <FormControl>
                        <FormLabel>Reacher Email:</FormLabel>
                        {edit ? <InputGroup>
                            <Input
                                placeholder="yourname"
                                value={reacherPrefix}
                                onChange={v => setSetupConfig(c => ({ ...c, reacherEmailPrefix: v.target.value }))}
                                required
                                autoComplete="off"
                                isInvalid={reacherPrefixTaken}
                                onBlur={checkReacherEmail}
                            />
                            <InputRightAddon children={reacherSuffix} />
                        </InputGroup> : <Text>{reacherPrefix}{reacherSuffix}</Text>}
                        {reacherPrefixInvalid ?
                            (<FormHelperText textColor="red.500">{setupConfig.reacherEmailPrefix}{reacherSuffix} is not a valid email prefix. Try something else</FormHelperText>) :
                            reacherPrefixTaken ? (<FormHelperText textColor="red.500">{setupConfig.reacherEmailPrefix}{reacherSuffix} is taken. Try something else</FormHelperText>) :
                                (<FormHelperText>This is your reacher email where people can pay a tip to reach you.</FormHelperText>)
                        }
                    </FormControl>
                    <FormControl>
                        <FormLabel>Display Name:</FormLabel>
                        {edit ? <Input
                            placeholder="Satoshi Nakamoto"
                            value={setupConfig!.name}
                            onChange={v => setSetupConfig(c => ({ ...c, name: v.target.value }))}
                            required
                            autoComplete="off"
                        /> : <Text>{setupConfig!.name}</Text>}
                        <FormHelperText>This is the name people will see when referring to you in emails we send.</FormHelperText>
                    </FormControl>
                    <FormControl>
                        <FormLabel>Strike Username:</FormLabel>
                        {edit ? <Input
                            placeholder="satoshi"
                            value={setupConfig!.strikeUsername}
                            onChange={v => setSetupConfig(c => ({ ...c, strikeUsername: v.target.value }))}
                            onBlur={() => updateCurrency()}
                            required
                            autoComplete="off"
                        /> : <Text>{setupConfig!.strikeUsername}</Text>}
                        {usernameInvalid ? (<FormHelperText textColor="red.300">We couldn't find this strike user, so there's a good chance the username is invalid.</FormHelperText>) :
                            (<FormHelperText>This is the the username of the Strike account that will receive the funds.</FormHelperText>)}
                    </FormControl>
                    <FormControl>
                        <FormLabel>Destination Email:</FormLabel>
                        {edit ? <Input
                            type="email"
                            placeholder="user@email.com"
                            value={setupConfig!.destinationEmail}
                            onChange={v => setSetupConfig(c => ({ ...c, destinationEmail: v.target.value }))}
                            required
                            autoComplete="off"
                        /> : <Text>{setupConfig!.destinationEmail}</Text>}
                        <FormHelperText>This is the inbox emails will go to once a tip has been paid.</FormHelperText>
                    </FormControl>
                    <FormControl>
                        <FormLabel>Tip Amount ({setupConfig.currency}):</FormLabel>
                        {edit ? <InputGroup>
                            <InputLeftAddon children={`$`} />
                            <Input
                                placeholder="2.00"
                                defaultValue={tipAmount}
                                onChange={v => {
                                    var value = parseFloat(v.target.value);
                                    if (value > 0) {
                                        setSetupConfig(c => ({ ...c, price: parseFloat(value.toFixed(2)) }))
                                    }
                                }}
                                required
                                autoComplete="off"
                            />
                        </InputGroup> : <Text>${tipAmount}</Text>}
                        <FormHelperText>This is the amount people must tip in order to get their emails delivered to your inbox.</FormHelperText>
                    </FormControl>
                    <FormControl>
                        <FormLabel htmlFor='is-disabled' mb='0'>
                            Enabled?
                        </FormLabel>
                        {edit ? <Switch id="is-disabled" isChecked={!setupConfig.disabled}
                            onChange={v => {
                                setSetupConfig(c => ({ ...c, disabled: !v.target.checked }))
                            }} /> : <Text color={setupConfig.disabled ? "red" : undefined}>{setupConfig.disabled ? "NO" : "YES"}</Text>}
                        <FormHelperText>You can disable your reacher email and nothing will happen if someone sends an email to that address. </FormHelperText>
                    </FormControl>
                    {edit && <FormControl>
                        <FormHelperText>By hitting 'Submit' below, you are agreeing to the {" "}
                            <Link color={orangeColor}
                                href="https://strike.me/en/legal/tos"
                                isExternal
                            >
                                Terms of Service
                            </Link>{" and "}
                            <Link color={orangeColor}
                                href="https://strike.me/en/legal/privacy"
                                isExternal
                            >
                                Privacy Policy
                            </Link></FormHelperText>
                    </FormControl>}
                    {error && <Text color="red">
                        An error occurred. Please try again or see console for details.
                    </Text>}
                    {isNew ? <Button isLoading={isSaving} disabled={isSaving} type="submit" width="100%">Submit</Button> : edit ? <SimpleGrid width="100%" columns={2} spacing={5}>
                        <Button width="100%" secondary onClick={() => onComplete()}>Cancel</Button>
                        <Button isLoading={isSaving} disabled={isSaving} type="submit" width="100%">Submit</Button>
                    </SimpleGrid> : <Button width="100%" onClick={onEdit}>Update Profile</Button>}
                </VStack>
            </form>
        </VStack>
    </>);
}
export default Setup;