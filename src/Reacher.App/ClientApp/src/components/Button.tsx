import { Button as ChakraButton, ButtonProps } from "@chakra-ui/react";
import * as React from 'react';

import { orangeColor } from './Common';
export const Button: React.FC<ButtonProps & { href?: string }> = ({
    children,
    onClick,
    href,
    ...rest
}) => {
    if (href && !onClick) {
        onClick = () => { window.location.href = href;}
    }
    return (
        <ChakraButton color="black" bg={orangeColor} _hover={{ opacity:0.6 }}
            onClick={onClick}
            variant="primary"
            {...rest}
        >
            {children}
        </ChakraButton>
    );
}

export default Button;