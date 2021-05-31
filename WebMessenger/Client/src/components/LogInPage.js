import React, {useState} from 'react';
import {useHistory} from "react-router-dom";

function LogInPage(props) {

    const defaultLoginExceptionMessage = 'Empty User Name';
    const defaultPasswordExceptionMessage = 'Empty Password';

    const defaultInputNameClass = 'input-text';


    const [labelPasswordClass, setPasswordClass] = useState('label-text');
    const [labelUserNameClass, setUserNameClass] = useState('label-text');
    const [passwordException, setPasswordException] = useState('wrong-input');
    const [loginException, setLoginException] = useState('wrong-input');
    const [needMovePassword, setNeedMovePassword] = useState(true);
    const [needMoveUser, setNeedMoveUser] = useState(true);
    const [userObj, setUserObj] = useState(new Object({login: '', password: ''}));
    const [loginExceptionMessage, setLoginExceptionMessage] = useState(defaultLoginExceptionMessage);
    const [nameInputClass, setNameInputClass] = useState(defaultInputNameClass);
    const [passwordExceptionMessage, setPasswordExceptionMessage] = useState(defaultPasswordExceptionMessage)
    const [passwordInputClass, setPasswordInputClass] = useState(defaultInputNameClass);
    const history = useHistory();

    const handlePasswordChange = (event) => {
        const value = event.target.value;
        setUserObj({...userObj, password: event.target.value});
        setNeedMovePassword(value === '');
    };

    const handleUserChange = (event) => {
        const value = event.target.value;
        setUserObj({...userObj, login: event.target.value});
        setNeedMoveUser(value === '');
    };


    const handleSubmit = async (event) => {
        event.preventDefault();
        if (!Object.values(userObj).includes('')) {
            const response = await fetch(`api/users/check?login=${userObj.login}&password=${userObj.password}`);
            if( response.status === 404){
                setLoginExceptionMessage('User don\'t exists')
                setLoginException('visible-wrong-input');
                setNameInputClass(`${defaultInputNameClass} bad-input`);
                return;
            }
            if( response.status === 400){
                setPasswordExceptionMessage('Wrong Password');
                setPasswordException('visible-wrong-input');
                setPasswordInputClass(`${defaultInputNameClass} bad-input`);
                return;
            }
            const id = await response.json();
            props.setCurrentUser(id);
            history.push('/messenger');
        } else {
            if (userObj.login === '') {
                setLoginException('visible-wrong-input');
                setUserNameClass('label-text bad-input')
            }
            if (userObj.password === '') {
                setPasswordException('visible-wrong-input');
                setPasswordClass('label-text bad-input');
            }
        }
    }

    const handleToSign = () => {
        history.push('/registration');
    }



    return (
        <div className="container">
            <div className="login-form-wrapper">
                <form className='form' method='GET' onSubmit={handleSubmit}>
                    <h3 className='sign-in-text'>Log In</h3>
                    <div>
                        <label className={labelUserNameClass} htmlFor="user-name">User Name</label>
                        <input className={nameInputClass} type="text" id="user-name" autoComplete="off"
                               onFocus={() => {
                                   setNameInputClass(defaultInputNameClass);
                                   setLoginExceptionMessage(defaultLoginExceptionMessage);
                                   setUserNameClass('label-text input-focused');
                                   setLoginException('wrong-input');
                               }}
                               onChange={handleUserChange}
                               onBlur={() => {
                                   if (needMoveUser) setUserNameClass('label-text')
                               }}/>
                        <p className={loginException}>{loginExceptionMessage}</p>
                    </div>
                    <div>
                        <label className={labelPasswordClass} htmlFor="password">Password</label>
                        <input className={passwordInputClass} type="text" id="password" autoComplete="off"
                               onFocus={() => {
                                   setPasswordInputClass(defaultInputNameClass);
                                   setPasswordExceptionMessage(defaultPasswordExceptionMessage);
                                   setPasswordClass('label-text input-focused')
                                   setPasswordException('wrong-input');
                               }}
                               onChange={handlePasswordChange}
                               onBlur={() => {
                                   if (needMovePassword) setPasswordClass('label-text');
                               }}/>
                        <p className={passwordException}>{passwordExceptionMessage}</p>
                    </div>
                    <div className='login-buttons-wrapper'>
                        <input type='submit' className='login-submit' value='Submit'/>
                        <button className='login-to-sign-button' onClick={handleToSign}>Sign in</button>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default LogInPage
