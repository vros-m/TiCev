import React from 'react';
import {
  Grid,
  Container
} from '@mui/material';
import { useLoaderData } from 'react-router-dom';
import VideoCard from '../GeneralComponents/VideoCard';

export async function HomeLoader({params})
{
    const array = new Array(33).fill(1)
    return array
}

export default function Home() {
    const data = useLoaderData()
    const display = data.map(item=><VideoCard/>)
    return (
      <Container sx={{marginTop:'110px'}}>
        <Grid container spacing={3}>
            {React.Children.map(display, (child, index) => (
            <React.Fragment key={index}>{child}</React.Fragment>
            ))}
      </Grid>
      </Container>
    );
  };