import React from 'react';
import {useDump} from "../bin/Hooks";

const Home = () => {
    const model = useDump();
    return (
        <div>
            Home<br />
            <code>{model}</code>
        </div>
    );
};

export default Home;