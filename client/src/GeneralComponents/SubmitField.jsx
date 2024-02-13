import React from 'react';
import { useState } from 'react';
import { TextField, InputAdornment, IconButton } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';

const SubmitField = ({ onSubmit,icon,label,width,transformation='' }) => {
  const handleSubmit = (e) => {
        onSubmit(transformation+state);
    };
    function handleChange(ev)
    {
        setState(ev.target.value)
    }

    const [state,setState] = useState('')
  return (
    <TextField
      label={label}
      variant="outlined"
      fullWidth
      sx={{
        width:{width}
      }}
      InputProps={{
        endAdornment: (
          <InputAdornment position="end">
            <IconButton onClick={handleSubmit} edge="end">
              {icon}
            </IconButton>
          </InputAdornment>
        ),
      }}
          value={state}
          onChange={handleChange}
    />
  );
};

export default SubmitField;