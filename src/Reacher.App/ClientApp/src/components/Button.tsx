import { Button as ChakraButton, ButtonProps } from "@chakra-ui/react";
import * as React from 'react';

export const Button: React.FC<ButtonProps> = ({
    children,
    onClick = () => { },
    ...rest
}) => {
    return (
        <ChakraButton
            onClick={onClick}
            variant="primary"
            height={14}
            {...rest}
        >
            {children}
        </ChakraButton>
    );
}

export default Button; 