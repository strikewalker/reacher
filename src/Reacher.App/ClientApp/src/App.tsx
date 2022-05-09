import * as React from 'react';
import { Route } from 'react-router';
import Home from './components/Home';
import Invoice from './components/Invoice';
import MyReacher from './components/MyReacher';
import { ConfigProvider } from 'react-avatar';
import {
    ChakraProvider,
    extendTheme,
    theme as base
} from "@chakra-ui/react"

import './custom.css'
import { mode } from "@chakra-ui/theme-tools";

// 2. Add your color mode config
const config = {
    initialColorMode: 'dark',
    useSystemColorMode: false,
    fonts: {
        heading: `Montserrat, ${base.fonts?.heading}`,
        body: `Montserrat, ${base.fonts?.body}`,
    }
}

// 3. extend the theme
const theme = extendTheme({
    config,
    styles: {
        global: (props: any) => ({
            body: {
                bg: mode("white", "black")(props),
            }
        })
    }
});


export default class App extends React.Component {
    static displayName = App.name;

    render() {
        return (
            <ConfigProvider avatarRedirectUrl="https://avatar-redirect.appspot.com">
                <ChakraProvider theme={theme}>
                    <Route exact path='/' component={Home} />
                    <Route path='/tip/:id' component={Invoice} />
                    <Route path='/myreacher' component={MyReacher} />
                </ChakraProvider>
            </ConfigProvider>
        );
    }
}
