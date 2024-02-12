import TextField from '@mui/material/TextField';
import { FormControl,FormControlLabel,RadioGroup,FormLabel,Radio } from '@mui/material';
import Button from '@mui/material/Button';
import Alert from '@mui/material/Alert'
import './Login.css'
import { Link, useNavigate } from 'react-router-dom';
import { userController } from '../../Constants';
import { useContext, useState } from 'react';
import { login } from './Login';
import UserContext from '../Contexts/UserContext';

function loginFailed(setLoginState,msg)
{
    setLoginState((oldValue) =>
    {
        setTimeout(setLoginState({
            ...oldValue
        }),3000)
        return {
            ...oldValue,flag:!oldValue.flag,msg
        }
    })
}

export function loginUser(userState,setUserState,navigate)
{
    setUserState(userState)
    navigate('/')
}

async function signup(signUpState,navigate,setLoginState,setUserState)
{
    const response = await fetch(userController + '/RegisterUser', {
        method:'POST',
        headers: {
            "Content-Type":"application/json"
        },
        body: JSON.stringify({
            ...signUpState,
            birthday:new Date(signUpState.birthday).toISOString()
        })
    })

    if (response.status === 200)
    {
        login(signUpState.username,signUpState.password,navigate,setLoginState,setUserState)
    }
    else
    {
        loginFailed(setLoginState,await response.text())
    }
}

export default function Signup() { 

    const [signupFailedState, setSignupFailedState] = useState({flag:false,msg:''})
    const navigate = useNavigate()
    const [signupState,setSignupState] = useState({
        username:"",password:"",email:"",bio:"",profilePicture:"",name:"",birthday:"",gender:'male'
    })
    const [userState,setUserState] = useContext(UserContext)

    function onChange(event)
    {
        setSignupState((oldValue) =>
        {
            return {
                ...oldValue,
                [event.target.name]:event.target.value
            }

        })
    }

    function onFileUpload(event)
    {
        const file = event.target.files[0]
        if (file == null) return
        const fileReader = new FileReader()
        fileReader.readAsDataURL(file)
        fileReader.onload=(ev) =>
        {
            const size =new TextEncoder().encode(fileReader.result).length
            if (size >= 15 * 1024 * 1024)
            {
                alert("Image too large!")
                return
            }
            alert("Image uploaded.")
            setSignupState(oldValue => {
                return {
                    ...oldValue,
                    profilePicture: fileReader.result
                }
            })
        }
    }

    return <div className="login_container">
        <h2>Signup</h2>
        <TextField id="Name" label="Name" variant="outlined" sx={{
            marginTop:"10px",
            marginBottom: "10px"
            
        }}
            value={signupState.name}
            name='name'
        onChange={(onChange)}/>
        <TextField id="Username" label="Username" variant="outlined" sx={{
            marginBottom: "10px"
            
        }}
            value={signupState.username}
            name='username'
        onChange={(onChange)}/>
        <TextField id="Password" label="Password" variant="outlined" type='password'
            sx={{
                marginBottom:"10px"
            }}
            value={signupState.password}
            name='password'
            onChange={(onChange)} />
        <TextField id="Email" label="Email" variant="outlined" type='email'
            sx={{
                marginBottom:"10px"
            }}
            value={signupState.email}
            name='email'
            onChange={(onChange)} />
        <TextField id="Bio" label="Bio" variant="outlined" type='text'
            sx={{
                marginBottom:"10px"
            }}
            value={signupState.bio}
            name='bio'
            onChange={(onChange)} />
        <TextField
            id='Birthday'
          label="Birthday"
          variant="outlined"
            type="date"
            name='birthday'
          value={signupState.birthday}
            onChange={(onChange)}
            sx={{
                marginBottom: "10px",
                width:'224px'
            }}
            InputLabelProps={{shrink:true}}
        />
         <FormControl component="fieldset" margin="normal" fullWidth sx={{width:'220px'}}>
          <FormLabel component="legend">Gender</FormLabel>
            <RadioGroup
                row
            aria-label="gender"
            name="gender"
            value={signupState.gender}
            onChange={onChange}
                >
            <FormControlLabel value="male" control={<Radio />} label="Male" style={{color:'gray'} } />
            <FormControlLabel value="female" control={<Radio />} label="Female"  style={{color:'gray'} }/>
          </RadioGroup>
        </FormControl>
        <label htmlFor='input_file'>Add profile picture.</label> <input id='input_file' type='file' onChange={onFileUpload}
        accept=".jpg, .jpeg, .png" ></input>
        {signupFailedState.flag && <Alert severity="error">Signup failed. Reason: {signupFailedState.msg}</Alert>}
        <Button variant="text" sx={{
            width:"80px"
        }} id='Button' onClick={(ev) =>
        {
            signup(signupState, navigate, setSignupFailedState,setUserState)
        }}>OK</Button>
        <Link to='/login'>Already have an account? Log in.</Link>
       
    </div>
}
