import React from 'react';
// import {useDump} from "../bin/Hooks";

const Home = () => {
    const model = 'meh' // useDump();
    return (
        <div>
            Home<br />
            <code>{model}</code>
        </div>
    );
};

export default Home;