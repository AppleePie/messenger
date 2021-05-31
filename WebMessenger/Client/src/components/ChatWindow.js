import React, {useEffect, useState} from 'react';

function ChatWindow(props){
    return(
        <div className='chat-window'>
            {
                props.user === '' ? null : null
            }
        </div>
    )
}

export default ChatWindow