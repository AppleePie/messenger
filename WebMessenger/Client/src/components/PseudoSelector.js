import React, {useEffect, useState} from 'react';


function PseudoSelector(props){

    const handleClick = async (login) => {
        props.setIsLoadingOrHidden(true);
        // props.setIsChoseNewDialogue(true);
        const response = await fetch('/api/chats', {
            method: 'POST',
            body: JSON.stringify({
                initiatorId:  props.currentuser,
                interlocutorName: login
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }

    const renderUserButton = (user) => {
        return (
            <button className='found-user' key={user.id} onClick={() => handleClick(user.login)}>{user.login}</button>
        )
    };

    return(
        <div className='pseudoSelector'>
            {
                props.foundUsers.map(renderUserButton)
            }
        </div>
    )
}

export default PseudoSelector