function PseudoSelector(props) {
    const defaultImage = "/icons/default-photo.jpg";


    const handleClick = async (user) => {
        props.setIsHidden(true);
        const responseAvatar = await fetch(`api/users/${user.id}/avatar`);
        const avatar = responseAvatar.status === 404 ? defaultImage : await responseAvatar.text();
        props.setCurrentInterlocutor({avatar: avatar, login: user.login, interlocutor: user.id, scroll:props.users});
        props.setCurrentChatId('');
    }

    const renderUserButton = (user) => {
        return (
            <button className='found-user' key={user.id} onClick={() => handleClick(user)}>{user.login}</button>
        )
    };

    return (
        <div className='pseudoSelector'>
            {
                props.foundUsers.map(renderUserButton)
            }
        </div>
    )
}

export default PseudoSelector