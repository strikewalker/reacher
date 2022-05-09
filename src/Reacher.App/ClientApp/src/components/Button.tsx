import { Button as ChakraButton, ButtonProps } from "@chakra-ui/react";
import * as React from 'react';

import { orangeColor } from './Common';
export const Button: React.FC<ButtonProps & { href?: string, secondary?: boolean }> = ({
    children,
    onClick,
    href,
    secondary,
    ...rest
}) => {
    if (href && !onClick) {
        onClick = () => { window.location.href = href;}
    }
    const props: ButtonProps = secondary ? { borderColor: orangeColor, color: orangeColor, variant: "outline" } : { backgroundColor: orangeColor, color: "black" };
    return (
        <ChakraButton colorScheme="orange" _hover={{ opacity: 0.6 }}
            {...props}
            onClick={onClick}
            {...rest}
        >
            {children}
        </ChakraButton>
    );
}

export default Button;