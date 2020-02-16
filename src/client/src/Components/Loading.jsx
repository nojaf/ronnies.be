import React from 'react';
import PropTypes from 'prop-types'

const Loading = ({info}) => {
    return (
        <div id={'preloader'}>
            <div id="loader">{info}</div>
        </div>
    );
};

Loading.propTypes = {
    info: PropTypes.string
}

export default Loading;